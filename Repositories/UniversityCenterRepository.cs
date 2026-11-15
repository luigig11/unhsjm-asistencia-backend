using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class UniversityCenterRepository : IUniversityCenterRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public UniversityCenterRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<UniversityCenterEntity>> GetAllAsync(
            string? search,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                id AS Id,
                codigo AS Code,
                nombre AS Name,
                ubicacion AS Location,
                activo AS IsActive
            FROM public.centro_universitario
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(search))
            {
                sqlBuilder.Append(" AND (codigo ILIKE @Search OR nombre ILIKE @Search)");
                parameters.Add("Search", $"%{search.Trim()}%");
            }

            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY nombre ASC;");

            var items = await _dbContext.QueryAsync<UniversityCenterEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<UniversityCenterEntity> CreateAsync(UniversityCenterEntity center, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.centro_universitario (codigo, nombre, ubicacion, activo)
            VALUES (@Code, @Name, @Location, @IsActive)
            RETURNING id AS Id, codigo AS Code, nombre AS Name, ubicacion AS Location, activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<UniversityCenterEntity>(
                sql,
                new { center.Code, center.Name, center.Location, center.IsActive },
                cancellationToken: cancellationToken);

            return created!;
        }

        public async Task<UniversityCenterEntity?> UpdateAsync(Guid id, UniversityCenterEntity center, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.centro_universitario
            SET codigo = @Code, nombre = @Name, ubicacion = @Location, activo = @IsActive
            WHERE id = @Id
            RETURNING id AS Id, codigo AS Code, nombre AS Name, ubicacion AS Location, activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<UniversityCenterEntity>(
                sql,
                new { Id = id, center.Code, center.Name, center.Location, center.IsActive },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.centro_universitario WHERE id = @Id;";
            var affected = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affected > 0;
        }
    }
}