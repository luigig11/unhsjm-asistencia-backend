using Dapper;
using System.Text;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class DepartmentRepository : IDepartmentRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public DepartmentRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<DepartmentEntity>> GetAllAsync(
            string? search,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                id AS Id,
                codigo AS Code,
                nombre AS Name,
                activo AS IsActive
            FROM public.direccion
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

            var items = await _dbContext.QueryAsync<DepartmentEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<DepartmentEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                id AS Id,
                codigo AS Code,
                nombre AS Name,
                activo AS IsActive
            FROM public.direccion
            WHERE id = @Id;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<DepartmentEntity>(sql, new { Id = id }, cancellationToken: cancellationToken);
        }

        public async Task<DepartmentEntity> CreateAsync(DepartmentEntity department, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.direccion (codigo, nombre, activo)
            VALUES (@Code, @Name, @IsActive)
            RETURNING id AS Id, codigo AS Code, nombre AS Name, activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<DepartmentEntity>(
                sql,
                new { department.Code, department.Name, department.IsActive },
                cancellationToken: cancellationToken);

            return created!;
        }

        public async Task<DepartmentEntity?> UpdateAsync(Guid id, DepartmentEntity department, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.direccion
            SET codigo = @Code, nombre = @Name, activo = @IsActive
            WHERE id = @Id
            RETURNING id AS Id, codigo AS Code, nombre AS Name, activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<DepartmentEntity>(
                sql,
                new { Id = id, department.Code, department.Name, department.IsActive },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.direccion WHERE id = @Id;";
            var affected = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affected > 0;
        }
    }
}
