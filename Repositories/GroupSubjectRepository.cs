using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class GroupSubjectRepository : IGroupSubjectRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public GroupSubjectRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<GroupSubjectWithDetailsEntity>> GetAllAsync(
            Guid? groupId,
            Guid? teacherId,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                mpg.id AS Id,
                mpg.id_grupo AS GroupId,
                g.codigo AS GroupCode,
                mpg.id_malla_curricular AS CurricularGridItemId,
                mc.id_materia AS SubjectId,
                m.codigo AS SubjectCode,
                m.nombre AS SubjectName,
                mpg.id_docente AS TeacherId,
                CONCAT(u.nombre, ' ', u.apellido) AS TeacherFullName,
                mpg.cupo_limite AS QuotaLimit,
                COUNT(ig.id)::int AS TotalEnrolled,
                mpg.activo AS IsActive
            FROM public.materia_por_grupo mpg
            INNER JOIN public.grupo g ON g.id = mpg.id_grupo
            INNER JOIN public.malla_curricular mc ON mc.id = mpg.id_malla_curricular
            INNER JOIN public.materia m ON m.id = mc.id_materia
            INNER JOIN public.docente d ON d.id = mpg.id_docente
            INNER JOIN public.usuario u ON u.id = d.id_usuario
            LEFT JOIN public.inscripcion_grupo ig ON ig.id_materia_por_grupo = mpg.id AND ig.activo = TRUE
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();

            if (groupId.HasValue)
            {
                sqlBuilder.Append(" AND mpg.id_grupo = @GroupId");
                parameters.Add("GroupId", groupId.Value);
            }

            if (teacherId.HasValue)
            {
                sqlBuilder.Append(" AND mpg.id_docente = @TeacherId");
                parameters.Add("TeacherId", teacherId.Value);
            }

            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND mpg.activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append("""
             GROUP BY
                mpg.id,
                mpg.id_grupo,
                g.codigo,
                mpg.id_malla_curricular,
                mc.id_materia,
                m.codigo,
                m.nombre,
                mpg.id_docente,
                u.nombre,
                u.apellido,
                mpg.cupo_limite,
                mpg.activo
             ORDER BY g.codigo ASC, m.nombre ASC;
            """);

            var items = await _dbContext.QueryAsync<GroupSubjectWithDetailsEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<GroupSubjectGenerationContextEntity?> GetGenerationContextAsync(
            Guid groupSubjectId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                mpg.id AS GroupSubjectId,
                ia.id_periodo_escolar AS SchoolPeriodId,
                ia.fecha_inicio AS InstanceStartDate,
                ia.fecha_fin AS InstanceEndDate
            FROM public.materia_por_grupo mpg
            INNER JOIN public.grupo g ON g.id = mpg.id_grupo
            INNER JOIN public.instancia_academica ia ON ia.id = g.id_instancia_academica
            WHERE mpg.id = @GroupSubjectId;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<GroupSubjectGenerationContextEntity>(
                sql,
                new { GroupSubjectId = groupSubjectId },
                cancellationToken: cancellationToken);
        }

        public async Task<GroupSubjectEntity> CreateAsync(GroupSubjectEntity groupSubject, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.materia_por_grupo (id_grupo, id_malla_curricular, id_docente, cupo_limite, activo)
            VALUES (@GroupId, @CurricularGridItemId, @TeacherId, @QuotaLimit, @IsActive)
            RETURNING
                id AS Id,
                id_grupo AS GroupId,
                id_malla_curricular AS CurricularGridItemId,
                id_docente AS TeacherId,
                cupo_limite AS QuotaLimit,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<GroupSubjectEntity>(sql, groupSubject, cancellationToken: cancellationToken);
            return created!;
        }

        public async Task<GroupSubjectEntity?> UpdateAsync(
            Guid id,
            Guid teacherId,
            short quotaLimit,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.materia_por_grupo
            SET
                id_docente = @TeacherId,
                cupo_limite = @QuotaLimit,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                id_grupo AS GroupId,
                id_malla_curricular AS CurricularGridItemId,
                id_docente AS TeacherId,
                cupo_limite AS QuotaLimit,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<GroupSubjectEntity>(
                sql,
                new { Id = id, TeacherId = teacherId, QuotaLimit = quotaLimit, IsActive = isActive },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.materia_por_grupo WHERE id = @Id;";
            var affectedRows = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affectedRows > 0;
        }
    }
}