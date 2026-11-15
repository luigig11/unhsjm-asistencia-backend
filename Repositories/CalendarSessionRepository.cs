using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class CalendarSessionRepository : ICalendarSessionRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public CalendarSessionRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<CalendarSessionWithDetailsEntity>> GetAllAsync(
            Guid? groupSubjectId,
            DateTimeOffset? dayStartUtc,
            DateTimeOffset? dayEndUtc,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                c.id AS Id,
                c.id_materia_por_grupo AS GroupSubjectId,
                c.id_aula AS ClassroomId,
                a.codigo AS ClassroomCode,
                c.fecha_inicio AS StartDate,
                c.fecha_fin AS EndDate,
                c.activo AS IsActive,
                CASE WHEN ast.id IS NOT NULL THEN 'REGISTRADA' ELSE 'PENDIENTE' END AS AttendanceStatus
            FROM public.calendario c
            INNER JOIN public.aula a ON a.id = c.id_aula
            LEFT JOIN public.asistencia ast ON ast.id_calendario = c.id
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();

            if (groupSubjectId.HasValue)
            {
                sqlBuilder.Append(" AND c.id_materia_por_grupo = @GroupSubjectId");
                parameters.Add("GroupSubjectId", groupSubjectId.Value);
            }

            if (dayStartUtc.HasValue && dayEndUtc.HasValue)
            {
                sqlBuilder.Append(" AND c.fecha_inicio >= @DayStartUtc AND c.fecha_inicio < @DayEndUtc");
                parameters.Add("DayStartUtc", dayStartUtc.Value);
                parameters.Add("DayEndUtc", dayEndUtc.Value);
            }

            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND c.activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY c.fecha_inicio ASC;");

            var items = await _dbContext.QueryAsync<CalendarSessionWithDetailsEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<CalendarSessionEntity> CreateAsync(CalendarSessionEntity session, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.calendario (id_materia_por_grupo, id_aula, fecha_inicio, fecha_fin, activo)
            VALUES (@GroupSubjectId, @ClassroomId, @StartDate, @EndDate, @IsActive)
            RETURNING
                id AS Id,
                id_materia_por_grupo AS GroupSubjectId,
                id_aula AS ClassroomId,
                fecha_inicio AS StartDate,
                fecha_fin AS EndDate,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<CalendarSessionEntity>(sql, session, cancellationToken: cancellationToken);
            return created!;
        }

        public async Task<CalendarSessionEntity?> TryInsertIfNotExistsAsync(
            CalendarSessionEntity session,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.calendario (id_materia_por_grupo, id_aula, fecha_inicio, fecha_fin, activo)
            VALUES (@GroupSubjectId, @ClassroomId, @StartDate, @EndDate, @IsActive)
            ON CONFLICT (id_materia_por_grupo, fecha_inicio) DO NOTHING
            RETURNING
                id AS Id,
                id_materia_por_grupo AS GroupSubjectId,
                id_aula AS ClassroomId,
                fecha_inicio AS StartDate,
                fecha_fin AS EndDate,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<CalendarSessionEntity>(sql, session, cancellationToken: cancellationToken);
        }

        public async Task<CalendarSessionEntity?> UpdateAsync(
            Guid id,
            Guid classroomId,
            DateTimeOffset startDate,
            DateTimeOffset endDate,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.calendario
            SET
                id_aula = @ClassroomId,
                fecha_inicio = @StartDate,
                fecha_fin = @EndDate,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                id_materia_por_grupo AS GroupSubjectId,
                id_aula AS ClassroomId,
                fecha_inicio AS StartDate,
                fecha_fin AS EndDate,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<CalendarSessionEntity>(
                sql,
                new
                {
                    Id = id,
                    ClassroomId = classroomId,
                    StartDate = startDate,
                    EndDate = endDate,
                    IsActive = isActive
                },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.calendario WHERE id = @Id;";
            var affectedRows = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affectedRows > 0;
        }
    }
}