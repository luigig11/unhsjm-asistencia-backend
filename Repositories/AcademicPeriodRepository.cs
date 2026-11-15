using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class AcademicPeriodRepository : IAcademicPeriodRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public AcademicPeriodRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<AcademicPeriodWithTypeEntity>> GetAllAsync(
            Guid? academicPeriodTypeId,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                pa.id AS Id,
                pa.id_tipo_periodo_academico AS AcademicPeriodTypeId,
                tpa.nombre AS AcademicPeriodTypeName,
                pa.codigo AS Code,
                pa.nombre AS Name,
                pa.orden AS Order,
                pa.activo AS IsActive
            FROM public.periodo_academico pa
            INNER JOIN public.tipo_periodo_academico tpa ON tpa.id = pa.id_tipo_periodo_academico
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();

            if (academicPeriodTypeId.HasValue)
            {
                sqlBuilder.Append(" AND pa.id_tipo_periodo_academico = @AcademicPeriodTypeId");
                parameters.Add("AcademicPeriodTypeId", academicPeriodTypeId.Value);
            }

            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND pa.activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY pa.orden ASC;");

            var items = await _dbContext.QueryAsync<AcademicPeriodWithTypeEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<AcademicPeriodEntity> CreateAsync(AcademicPeriodEntity period, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.periodo_academico (
                id_tipo_periodo_academico, codigo, nombre, orden, activo
            )
            VALUES (
                @AcademicPeriodTypeId, @Code, @Name, @Order, @IsActive
            )
            RETURNING
                id AS Id,
                id_tipo_periodo_academico AS AcademicPeriodTypeId,
                codigo AS Code,
                nombre AS Name,
                orden AS Order,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<AcademicPeriodEntity>(
                sql,
                new { period.AcademicPeriodTypeId, period.Code, period.Name, period.Order, period.IsActive },
                cancellationToken: cancellationToken);

            return created!;
        }

        public async Task<AcademicPeriodEntity?> UpdateAsync(Guid id, AcademicPeriodEntity period, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.periodo_academico
            SET
                id_tipo_periodo_academico = @AcademicPeriodTypeId,
                codigo = @Code,
                nombre = @Name,
                orden = @Order,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                id_tipo_periodo_academico AS AcademicPeriodTypeId,
                codigo AS Code,
                nombre AS Name,
                orden AS Order,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<AcademicPeriodEntity>(
                sql,
                new { Id = id, period.AcademicPeriodTypeId, period.Code, period.Name, period.Order, period.IsActive },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.periodo_academico WHERE id = @Id;";
            var affected = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affected > 0;
        }
    }
}

