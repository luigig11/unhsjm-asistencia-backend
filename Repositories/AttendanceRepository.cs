using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class AttendanceRepository : IAttendanceRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public AttendanceRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DashboardSchoolPeriodEntity?> GetSchoolPeriodForDashboardAsync(
            Guid? schoolPeriodId,
            CancellationToken cancellationToken = default)
        {
            if (schoolPeriodId.HasValue)
            {
                const string sqlById = """
                SELECT
                    id AS Id,
                    nombre_periodo_escolar AS Name
                FROM public.periodo_escolar
                WHERE id = @Id;
                """;

                return await _dbContext.QueryFirstOrDefaultAsync<DashboardSchoolPeriodEntity>(
                    sqlById,
                    new { Id = schoolPeriodId.Value },
                    cancellationToken: cancellationToken);
            }

            const string sqlLatestActive = """
            SELECT
                id AS Id,
                nombre_periodo_escolar AS Name
            FROM public.periodo_escolar
            WHERE activo = TRUE
            ORDER BY fecha_inicio DESC
            LIMIT 1;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<DashboardSchoolPeriodEntity>(
                sqlLatestActive,
                cancellationToken: cancellationToken);
        }

        public async Task<int> CountOfferedGroupsAsync(Guid schoolPeriodId, CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT COUNT(*)::int
            FROM public.grupo g
            INNER JOIN public.instancia_academica ia ON ia.id = g.id_instancia_academica
            WHERE ia.id_periodo_escolar = @SchoolPeriodId
              AND ia.activo = TRUE
              AND g.activo = TRUE;
            """;

            return await _dbContext.ExecuteScalarAsync<int>(
                sql,
                new { SchoolPeriodId = schoolPeriodId },
                cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<DashboardTodaySessionEntity>> GetDashboardTodaySessionsAsync(
            Guid schoolPeriodId,
            DateTimeOffset dayStartUtc,
            DateTimeOffset dayEndUtc,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                c.id AS CalendarSessionId,
                m.nombre AS SubjectName,
                g.codigo AS GroupCode,
                a.codigo AS ClassroomCode,
                CONCAT(u.nombre, ' ', u.apellido) AS TeacherFullName,
                c.fecha_inicio AS StartDate,
                c.fecha_fin AS EndDate,
                CASE
                    WHEN ast.id IS NULL THEN 'PENDIENTE'
                    WHEN ast.docente_asignado_asistio = TRUE THEN 'PRESENTE'
                    WHEN ast.id_docente_sustituto IS NOT NULL THEN 'SUSTITUTO'
                    ELSE 'AUSENTE'
                END AS AttendanceStatus
            FROM public.calendario c
            INNER JOIN public.materia_por_grupo mpg ON mpg.id = c.id_materia_por_grupo
            INNER JOIN public.grupo g ON g.id = mpg.id_grupo
            INNER JOIN public.instancia_academica ia ON ia.id = g.id_instancia_academica
            INNER JOIN public.malla_curricular mc ON mc.id = mpg.id_malla_curricular
            INNER JOIN public.materia m ON m.id = mc.id_materia
            INNER JOIN public.aula a ON a.id = c.id_aula
            INNER JOIN public.docente d ON d.id = mpg.id_docente
            INNER JOIN public.usuario u ON u.id = d.id_usuario
            LEFT JOIN public.asistencia ast ON ast.id_calendario = c.id
            WHERE ia.id_periodo_escolar = @SchoolPeriodId
              AND c.activo = TRUE
              AND c.fecha_inicio >= @DayStartUtc
              AND c.fecha_inicio < @DayEndUtc
            ORDER BY c.fecha_inicio ASC, m.nombre ASC;
            """;

            var rows = await _dbContext.QueryAsync<DashboardTodaySessionEntity>(
                sql,
                new { SchoolPeriodId = schoolPeriodId, DayStartUtc = dayStartUtc, DayEndUtc = dayEndUtc },
                cancellationToken: cancellationToken);

            return rows.ToList();
        }

        public async Task<IReadOnlyList<DashboardUpcomingEventEntity>> GetUpcomingEventsAsync(
            Guid schoolPeriodId,
            DateTimeOffset fromUtc,
            int limit = 10,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                f.id AS HolidayId,
                f.nombre AS Name,
                te.nombre AS EventTypeName,
                f.fecha_inicio AS StartDate,
                f.fecha_fin AS EndDate
            FROM public.feriado f
            INNER JOIN public.tipo_evento te ON te.id = f.id_tipo_evento
            WHERE f.id_periodo_escolar = @SchoolPeriodId
              AND f.activo = TRUE
              AND f.fecha_fin >= @FromUtc
            ORDER BY f.fecha_inicio ASC
            LIMIT @Limit;
            """;

            var rows = await _dbContext.QueryAsync<DashboardUpcomingEventEntity>(
                sql,
                new { SchoolPeriodId = schoolPeriodId, FromUtc = fromUtc, Limit = limit },
                cancellationToken: cancellationToken);

            return rows.ToList();
        }

        public async Task<IReadOnlyList<TeacherTodaySessionEntity>> GetTeacherTodaySessionsAsync(
            Guid teacherId,
            DateTimeOffset dayStartUtc,
            DateTimeOffset dayEndUtc,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                c.id AS CalendarSessionId,
                c.id_materia_por_grupo AS GroupSubjectId,
                m.codigo AS SubjectCode,
                m.nombre AS SubjectName,
                g.codigo AS GroupCode,
                a.codigo AS ClassroomCode,
                a.ubicacion AS ClassroomLocation,
                c.fecha_inicio AS StartDate,
                c.fecha_fin AS EndDate,
                ast.id AS AttendanceId,
                (ast.id IS NOT NULL) AS IsRegistered,
                (mpg.id_docente <> @TeacherId AND ast.id_docente_sustituto = @TeacherId) AS IsSubstitute
            FROM public.calendario c
            INNER JOIN public.materia_por_grupo mpg ON mpg.id = c.id_materia_por_grupo
            INNER JOIN public.grupo g ON g.id = mpg.id_grupo
            INNER JOIN public.malla_curricular mc ON mc.id = mpg.id_malla_curricular
            INNER JOIN public.materia m ON m.id = mc.id_materia
            INNER JOIN public.aula a ON a.id = c.id_aula
            LEFT JOIN public.asistencia ast ON ast.id_calendario = c.id
            WHERE c.activo = TRUE
              AND c.fecha_inicio >= @DayStartUtc
              AND c.fecha_inicio < @DayEndUtc
              AND (mpg.id_docente = @TeacherId OR ast.id_docente_sustituto = @TeacherId)
            ORDER BY c.fecha_inicio ASC, m.nombre ASC;
            """;

            var rows = await _dbContext.QueryAsync<TeacherTodaySessionEntity>(
                sql,
                new { TeacherId = teacherId, DayStartUtc = dayStartUtc, DayEndUtc = dayEndUtc },
                cancellationToken: cancellationToken);

            return rows.ToList();
        }

        public async Task<IReadOnlyList<TeacherCourseEntity>> GetTeacherCoursesAsync(
            Guid teacherId,
            Guid? academicInstanceId,
            CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                mpg.id AS GroupSubjectId,
                ia.nombre AS AcademicInstanceName,
                g.codigo AS GroupCode,
                m.codigo AS SubjectCode,
                m.nombre AS SubjectName,
                COUNT(ig.id)::int AS TotalEnrolled,
                mpg.cupo_limite AS QuotaLimit
            FROM public.materia_por_grupo mpg
            INNER JOIN public.grupo g ON g.id = mpg.id_grupo
            INNER JOIN public.instancia_academica ia ON ia.id = g.id_instancia_academica
            INNER JOIN public.malla_curricular mc ON mc.id = mpg.id_malla_curricular
            INNER JOIN public.materia m ON m.id = mc.id_materia
            LEFT JOIN public.inscripcion_grupo ig ON ig.id_materia_por_grupo = mpg.id AND ig.activo = TRUE
            WHERE mpg.id_docente = @TeacherId
              AND mpg.activo = TRUE
              AND g.activo = TRUE
            """);

            var parameters = new DynamicParameters();
            parameters.Add("TeacherId", teacherId);

            if (academicInstanceId.HasValue)
            {
                sqlBuilder.Append(" AND g.id_instancia_academica = @AcademicInstanceId");
                parameters.Add("AcademicInstanceId", academicInstanceId.Value);
            }

            sqlBuilder.Append("""

            GROUP BY mpg.id, ia.nombre, g.codigo, m.codigo, m.nombre, mpg.cupo_limite
            ORDER BY ia.nombre ASC, g.codigo ASC, m.nombre ASC;
            """);

            var rows = await _dbContext.QueryAsync<TeacherCourseEntity>(
                sqlBuilder.ToString(),
                parameters,
                cancellationToken: cancellationToken);

            return rows.ToList();
        }

        public async Task<AttendanceSheetSessionEntity?> GetSessionSheetSessionAsync(
            Guid calendarSessionId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                c.id AS CalendarSessionId,
                c.id_materia_por_grupo AS GroupSubjectId,
                mpg.id_docente AS TitularTeacherId,
                m.codigo AS SubjectCode,
                m.nombre AS SubjectName,
                g.codigo AS GroupCode,
                a.codigo AS ClassroomCode,
                CONCAT(u.nombre, ' ', u.apellido) AS TitularTeacherFullName,
                c.fecha_inicio AS StartDate,
                c.fecha_fin AS EndDate
            FROM public.calendario c
            INNER JOIN public.materia_por_grupo mpg ON mpg.id = c.id_materia_por_grupo
            INNER JOIN public.grupo g ON g.id = mpg.id_grupo
            INNER JOIN public.malla_curricular mc ON mc.id = mpg.id_malla_curricular
            INNER JOIN public.materia m ON m.id = mc.id_materia
            INNER JOIN public.aula a ON a.id = c.id_aula
            INNER JOIN public.docente d ON d.id = mpg.id_docente
            INNER JOIN public.usuario u ON u.id = d.id_usuario
            WHERE c.id = @CalendarSessionId;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<AttendanceSheetSessionEntity>(
                sql,
                new { CalendarSessionId = calendarSessionId },
                cancellationToken: cancellationToken);
        }

        public async Task<AttendanceSheetHeaderEntity?> GetSessionSheetHeaderAsync(
            Guid calendarSessionId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                ast.id AS Id,
                ast.docente_asignado_asistio AS AssignedTeacherAttended,
                ast.id_docente_sustituto AS SubstituteTeacherId,
                CASE
                    WHEN u.id IS NOT NULL THEN CONCAT(u.nombre, ' ', u.apellido)
                    ELSE NULL
                END AS SubstituteTeacherFullName,
                ast.id_motivo_ausencia AS AbsenceReasonId,
                ast.fecha_registro_docente AS TeacherRecordedAt
            FROM public.asistencia ast
            LEFT JOIN public.docente ds ON ds.id = ast.id_docente_sustituto
            LEFT JOIN public.usuario u ON u.id = ds.id_usuario
            WHERE ast.id_calendario = @CalendarSessionId;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<AttendanceSheetHeaderEntity>(
                sql,
                new { CalendarSessionId = calendarSessionId },
                cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<AttendanceSheetStudentEntity>> GetSessionSheetStudentsAsync(
            Guid calendarSessionId,
            Guid groupSubjectId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                ad.id AS AttendanceDetailId,
                ig.id AS GroupEnrollmentId,
                e.id AS StudentId,
                e.codigo_unico AS UniqueCode,
                e.carnet AS StudentCarnet,
                CONCAT_WS(' ', e.nombres, e.primer_apellido, NULLIF(e.segundo_apellido, '')) AS FullName,
                COALESCE(ad.asistio, FALSE) AS Attended,
                ad.id_motivo_ausencia AS AbsenceReasonId,
                ad.fecha_registro_estudiante AS StudentRecordedAt
            FROM public.inscripcion_grupo ig
            INNER JOIN public.estudiante e ON e.id = ig.id_estudiante
            LEFT JOIN public.asistencia ast ON ast.id_calendario = @CalendarSessionId
            LEFT JOIN public.asistencia_detalle ad
                ON ad.id_asistencia = ast.id
               AND ad.id_inscripcion_grupo = ig.id
            WHERE ig.id_materia_por_grupo = @GroupSubjectId
              AND ig.activo = TRUE
            ORDER BY e.primer_apellido ASC, e.segundo_apellido ASC NULLS LAST, e.nombres ASC;
            """;

            var rows = await _dbContext.QueryAsync<AttendanceSheetStudentEntity>(
                sql,
                new { CalendarSessionId = calendarSessionId, GroupSubjectId = groupSubjectId },
                cancellationToken: cancellationToken);

            return rows.ToList();
        }

        public async Task<bool> TeacherExistsAndActiveAsync(Guid teacherId, CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM public.docente
                WHERE id = @TeacherId
                  AND activo = TRUE
            );
            """;

            return await _dbContext.ExecuteScalarAsync<bool>(
                sql,
                new { TeacherId = teacherId },
                cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyDictionary<Guid, AbsenceReasonEntity>> GetAbsenceReasonsByIdsAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken = default)
        {
            var distinctIds = ids.Distinct().ToArray();
            if (distinctIds.Length == 0)
            {
                return new Dictionary<Guid, AbsenceReasonEntity>();
            }

            const string sql = """
            SELECT
                id AS Id,
                codigo AS Code,
                nombre AS Name,
                descripcion AS Description,
                justifica_inasistencia AS JustifiesAbsence,
                aplica_a AS AppliesTo,
                activo AS IsActive
            FROM public.motivo_ausencia
            WHERE id = ANY(@Ids);
            """;

            var rows = await _dbContext.QueryAsync<AbsenceReasonEntity>(
                sql,
                new { Ids = distinctIds },
                cancellationToken: cancellationToken);

            return rows.ToDictionary(r => r.Id);
        }

        public async Task<IReadOnlySet<Guid>> GetActiveEnrollmentIdsForGroupSubjectAsync(
            Guid groupSubjectId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT id
            FROM public.inscripcion_grupo
            WHERE id_materia_por_grupo = @GroupSubjectId
              AND activo = TRUE;
            """;

            var ids = await _dbContext.QueryAsync<Guid>(
                sql,
                new { GroupSubjectId = groupSubjectId },
                cancellationToken: cancellationToken);

            return ids.ToHashSet();
        }

        public async Task<AttendanceEntity> UpsertAttendanceHeaderAsync(
            Guid calendarSessionId,
            bool assignedTeacherAttended,
            Guid? substituteTeacherId,
            Guid? absenceReasonId,
            DateTimeOffset teacherRecordedAtUtc,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.asistencia (
                id_calendario,
                id_docente_sustituto,
                docente_asignado_asistio,
                id_motivo_ausencia,
                fecha_registro_docente
            )
            VALUES (
                @CalendarSessionId,
                @SubstituteTeacherId,
                @AssignedTeacherAttended,
                @AbsenceReasonId,
                @TeacherRecordedAtUtc
            )
            ON CONFLICT (id_calendario) DO UPDATE
            SET
                id_docente_sustituto = EXCLUDED.id_docente_sustituto,
                docente_asignado_asistio = EXCLUDED.docente_asignado_asistio,
                id_motivo_ausencia = EXCLUDED.id_motivo_ausencia,
                fecha_registro_docente = EXCLUDED.fecha_registro_docente
            RETURNING
                id AS Id,
                id_calendario AS CalendarSessionId,
                id_docente_sustituto AS SubstituteTeacherId,
                docente_asignado_asistio AS AssignedTeacherAttended,
                id_motivo_ausencia AS AbsenceReasonId,
                fecha_registro_docente AS TeacherRecordedAt;
            """;

            var saved = await _dbContext.QueryFirstOrDefaultAsync<AttendanceEntity>(
                sql,
                new
                {
                    CalendarSessionId = calendarSessionId,
                    SubstituteTeacherId = substituteTeacherId,
                    AssignedTeacherAttended = assignedTeacherAttended,
                    AbsenceReasonId = absenceReasonId,
                    TeacherRecordedAtUtc = teacherRecordedAtUtc
                },
                cancellationToken: cancellationToken);

            return saved!;
        }

        public async Task UpsertAttendanceDetailsAsync(
            Guid attendanceId,
            IReadOnlyList<(Guid GroupEnrollmentId, bool Attended, Guid? AbsenceReasonId)> details,
            DateTimeOffset studentRecordedAtUtc,
            CancellationToken cancellationToken = default)
        {
            if (details.Count == 0)
            {
                return;
            }

            const string sql = """
            INSERT INTO public.asistencia_detalle (
                id_asistencia,
                id_inscripcion_grupo,
                asistio,
                id_motivo_ausencia,
                fecha_registro_estudiante
            )
            VALUES (
                @AttendanceId,
                @GroupEnrollmentId,
                @Attended,
                @AbsenceReasonId,
                @StudentRecordedAtUtc
            )
            ON CONFLICT (id_asistencia, id_inscripcion_grupo) DO UPDATE
            SET
                asistio = EXCLUDED.asistio,
                id_motivo_ausencia = EXCLUDED.id_motivo_ausencia,
                fecha_registro_estudiante = EXCLUDED.fecha_registro_estudiante;
            """;

            var parameters = details
                .Select(d => new
                {
                    AttendanceId = attendanceId,
                    d.GroupEnrollmentId,
                    d.Attended,
                    d.AbsenceReasonId,
                    StudentRecordedAtUtc = studentRecordedAtUtc
                })
                .ToArray();

            await _dbContext.ExecuteAsync(sql, parameters, cancellationToken: cancellationToken);
        }

        public async Task<AttendanceDetailEntity?> GetAttendanceDetailByIdAsync(
            Guid detailId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                id AS Id,
                id_asistencia AS AttendanceId,
                id_inscripcion_grupo AS GroupEnrollmentId,
                asistio AS Attended,
                id_motivo_ausencia AS AbsenceReasonId,
                fecha_registro_estudiante AS StudentRecordedAt
            FROM public.asistencia_detalle
            WHERE id = @Id;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<AttendanceDetailEntity>(
                sql,
                new { Id = detailId },
                cancellationToken: cancellationToken);
        }

        public async Task<AttendanceDetailEntity> UpdateAttendanceDetailAsync(
            Guid detailId,
            bool attended,
            Guid? absenceReasonId,
            DateTimeOffset studentRecordedAtUtc,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.asistencia_detalle
            SET
                asistio = @Attended,
                id_motivo_ausencia = @AbsenceReasonId,
                fecha_registro_estudiante = @StudentRecordedAtUtc
            WHERE id = @Id
            RETURNING
                id AS Id,
                id_asistencia AS AttendanceId,
                id_inscripcion_grupo AS GroupEnrollmentId,
                asistio AS Attended,
                id_motivo_ausencia AS AbsenceReasonId,
                fecha_registro_estudiante AS StudentRecordedAt;
            """;

            var updated = await _dbContext.QueryFirstOrDefaultAsync<AttendanceDetailEntity>(
                sql,
                new
                {
                    Id = detailId,
                    Attended = attended,
                    AbsenceReasonId = absenceReasonId,
                    StudentRecordedAtUtc = studentRecordedAtUtc
                },
                cancellationToken: cancellationToken);

            return updated!;
        }

        public async Task<bool> DeleteAttendanceByIdAsync(Guid attendanceId, CancellationToken cancellationToken = default)
        {
            const string sql = """
            DELETE FROM public.asistencia
            WHERE id = @Id;
            """;

            var affectedRows = await _dbContext.ExecuteAsync(
                sql,
                new { Id = attendanceId },
                cancellationToken: cancellationToken);

            return affectedRows > 0;
        }

        public async Task<RosterCourseHeaderEntity?> GetRosterCourseHeaderAsync(
            Guid groupSubjectId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                mpg.id AS GroupSubjectId,
                m.codigo AS SubjectCode,
                m.nombre AS SubjectName,
                g.codigo AS GroupCode,
                CONCAT(u.nombre, ' ', u.apellido) AS TeacherFullName,
                ia.nombre AS AcademicInstanceName
            FROM public.materia_por_grupo mpg
            INNER JOIN public.grupo g ON g.id = mpg.id_grupo
            INNER JOIN public.instancia_academica ia ON ia.id = g.id_instancia_academica
            INNER JOIN public.malla_curricular mc ON mc.id = mpg.id_malla_curricular
            INNER JOIN public.materia m ON m.id = mc.id_materia
            INNER JOIN public.docente d ON d.id = mpg.id_docente
            INNER JOIN public.usuario u ON u.id = d.id_usuario
            WHERE mpg.id = @GroupSubjectId;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<RosterCourseHeaderEntity>(
                sql,
                new { GroupSubjectId = groupSubjectId },
                cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<RosterSessionColumnEntity>> GetRosterSessionsAsync(
            Guid groupSubjectId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                c.id AS CalendarSessionId,
                c.fecha_inicio AS StartDate,
                (ast.id IS NOT NULL) AS HasAttendanceRecord
            FROM public.calendario c
            LEFT JOIN public.asistencia ast ON ast.id_calendario = c.id
            WHERE c.id_materia_por_grupo = @GroupSubjectId
              AND c.activo = TRUE
            ORDER BY c.fecha_inicio ASC;
            """;

            var rows = await _dbContext.QueryAsync<RosterSessionColumnEntity>(
                sql,
                new { GroupSubjectId = groupSubjectId },
                cancellationToken: cancellationToken);

            return rows.ToList();
        }

        public async Task<IReadOnlyList<RosterStudentRowEntity>> GetRosterStudentsAsync(
            Guid groupSubjectId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                ig.id AS GroupEnrollmentId,
                e.id AS StudentId,
                e.codigo_unico AS UniqueCode,
                e.carnet AS StudentCarnet,
                CONCAT_WS(' ', e.nombres, e.primer_apellido, NULLIF(e.segundo_apellido, '')) AS FullName
            FROM public.inscripcion_grupo ig
            INNER JOIN public.estudiante e ON e.id = ig.id_estudiante
            WHERE ig.id_materia_por_grupo = @GroupSubjectId
              AND ig.activo = TRUE
            ORDER BY e.primer_apellido ASC, e.segundo_apellido ASC NULLS LAST, e.nombres ASC;
            """;

            var rows = await _dbContext.QueryAsync<RosterStudentRowEntity>(
                sql,
                new { GroupSubjectId = groupSubjectId },
                cancellationToken: cancellationToken);

            return rows.ToList();
        }

        public async Task<IReadOnlyList<RosterMarkRowEntity>> GetRosterMarksAsync(
            Guid groupSubjectId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                ad.id_inscripcion_grupo AS GroupEnrollmentId,
                ast.id_calendario AS CalendarSessionId,
                ad.asistio AS Attended,
                ad.id_motivo_ausencia AS AbsenceReasonId,
                ma.nombre AS AbsenceReasonName,
                COALESCE(ma.justifica_inasistencia, FALSE) AS JustifiesAbsence,
                ad.fecha_registro_estudiante AS StudentRecordedAt
            FROM public.asistencia_detalle ad
            INNER JOIN public.asistencia ast ON ast.id = ad.id_asistencia
            INNER JOIN public.calendario c ON c.id = ast.id_calendario
            LEFT JOIN public.motivo_ausencia ma ON ma.id = ad.id_motivo_ausencia
            WHERE c.id_materia_por_grupo = @GroupSubjectId
              AND c.activo = TRUE;
            """;

            var rows = await _dbContext.QueryAsync<RosterMarkRowEntity>(
                sql,
                new { GroupSubjectId = groupSubjectId },
                cancellationToken: cancellationToken);

            return rows.ToList();
        }
    }
}