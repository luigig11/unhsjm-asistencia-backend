using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class GroupEnrollmentRepository : IGroupEnrollmentRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public GroupEnrollmentRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<GroupEnrollmentWithStudentEntity>> GetAllAsync(
            Guid? groupSubjectId,
            Guid? studentId,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                ig.id AS Id,
                ig.id_materia_por_grupo AS GroupSubjectId,
                ig.id_estudiante AS StudentId,
                e.codigo_unico AS UniqueCode,
                e.carnet AS StudentCarnet,
                e.nombres AS FirstNames,
                e.primer_apellido AS FirstLastName,
                e.segundo_apellido AS SecondLastName,
                ig.fecha_inscripcion AS EnrollmentDate,
                ig.activo AS IsActive
            FROM public.inscripcion_grupo ig
            INNER JOIN public.estudiante e ON e.id = ig.id_estudiante
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();

            if (groupSubjectId.HasValue)
            {
                sqlBuilder.Append(" AND ig.id_materia_por_grupo = @GroupSubjectId");
                parameters.Add("GroupSubjectId", groupSubjectId.Value);
            }

            if (studentId.HasValue)
            {
                sqlBuilder.Append(" AND ig.id_estudiante = @StudentId");
                parameters.Add("StudentId", studentId.Value);
            }

            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND ig.activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY e.primer_apellido ASC, e.nombres ASC;");

            var items = await _dbContext.QueryAsync<GroupEnrollmentWithStudentEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<GroupEnrollmentEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                id AS Id,
                id_materia_por_grupo AS GroupSubjectId,
                id_estudiante AS StudentId,
                fecha_inscripcion AS EnrollmentDate,
                activo AS IsActive
            FROM public.inscripcion_grupo
            WHERE id = @Id;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<GroupEnrollmentEntity>(sql, new { Id = id }, cancellationToken: cancellationToken);
        }

        public async Task<(short QuotaLimit, int ActiveCount)?> LockCourseAndCountActiveAsync(
            Guid groupSubjectId,
            CancellationToken cancellationToken = default)
        {
            const string lockSql = """
            SELECT cupo_limite
            FROM public.materia_por_grupo
            WHERE id = @GroupSubjectId
            FOR UPDATE;
            """;

            var quotaLimit = await _dbContext.QueryFirstOrDefaultAsync<short?>(
                lockSql,
                new { GroupSubjectId = groupSubjectId },
                cancellationToken: cancellationToken);

            if (!quotaLimit.HasValue)
            {
                return null;
            }

            const string countSql = """
            SELECT COUNT(*)::int
            FROM public.inscripcion_grupo
            WHERE id_materia_por_grupo = @GroupSubjectId AND activo = TRUE;
            """;

            var activeCount = await _dbContext.ExecuteScalarAsync<int>(
                countSql,
                new { GroupSubjectId = groupSubjectId },
                cancellationToken: cancellationToken);

            return (quotaLimit.Value, activeCount);
        }

        public async Task<GroupEnrollmentEntity> CreateAsync(GroupEnrollmentEntity enrollment, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.inscripcion_grupo (id_materia_por_grupo, id_estudiante, fecha_inscripcion, activo)
            VALUES (@GroupSubjectId, @StudentId, @EnrollmentDate, @IsActive)
            RETURNING
                id AS Id,
                id_materia_por_grupo AS GroupSubjectId,
                id_estudiante AS StudentId,
                fecha_inscripcion AS EnrollmentDate,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<GroupEnrollmentEntity>(sql, enrollment, cancellationToken: cancellationToken);
            return created!;
        }

        public async Task<GroupEnrollmentEntity?> UpdateStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.inscripcion_grupo
            SET activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                id_materia_por_grupo AS GroupSubjectId,
                id_estudiante AS StudentId,
                fecha_inscripcion AS EnrollmentDate,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<GroupEnrollmentEntity>(
                sql,
                new { Id = id, IsActive = isActive },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.inscripcion_grupo WHERE id = @Id;";
            var affectedRows = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affectedRows > 0;
        }
    }
}