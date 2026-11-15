using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class AcademicCycleRepository : IAcademicCycleRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public AcademicCycleRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<AcademicCycleEntity>> GetAllAsync(bool? isActive, CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                id AS Id,
                codigo AS Code,
                nombre AS Name,
                orden AS Order,
                activo AS IsActive
            FROM public.ciclo_academico
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();
            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY orden ASC;");

            var items = await _dbContext.QueryAsync<AcademicCycleEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<AcademicCycleEntity> CreateAsync(AcademicCycleEntity cycle, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.ciclo_academico (codigo, nombre, orden, activo)
            VALUES (@Code, @Name, @Order, @IsActive)
            RETURNING id AS Id, codigo AS Code, nombre AS Name, orden AS Order, activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<AcademicCycleEntity>(
                sql,
                new { cycle.Code, cycle.Name, cycle.Order, cycle.IsActive },
                cancellationToken: cancellationToken);

            return created!;
        }

        public async Task<AcademicCycleEntity?> UpdateAsync(Guid id, AcademicCycleEntity cycle, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.ciclo_academico
            SET codigo = @Code, nombre = @Name, orden = @Order, activo = @IsActive
            WHERE id = @Id
            RETURNING id AS Id, codigo AS Code, nombre AS Name, orden AS Order, activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<AcademicCycleEntity>(
                sql,
                new { Id = id, cycle.Code, cycle.Name, cycle.Order, cycle.IsActive },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.ciclo_academico WHERE id = @Id;";
            var affected = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affected > 0;
        }
    }
}

