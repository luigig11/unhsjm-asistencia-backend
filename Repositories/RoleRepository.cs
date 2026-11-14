using Dapper;
using System.Text;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class RoleRepository : IRoleRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public RoleRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<RoleEntity>> GetAllAsync(bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                id AS Id,
                code AS Code,
                nombre AS Name,
                activo AS IsActive
            FROM public.rol
            """);

            var parameters = new DynamicParameters();
            if (isActive.HasValue)
            {
                sqlBuilder.Append(" WHERE activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY code ASC;");

            var roles = await _dbContext.QueryAsync<RoleEntity>(
                sqlBuilder.ToString(),
                parameters,
                cancellationToken: cancellationToken);

            return roles.ToList();
        }

        public async Task<RoleEntity?> GetByIdAsync(Guid id, bool asAnonymous = false, CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                id AS Id,
                code AS Code,
                nombre AS Name,
                activo AS IsActive
            FROM public.rol
            WHERE id = @Id;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<RoleEntity>(
                sql,
                new { Id = id },
                asAnonymous: asAnonymous,
                cancellationToken: cancellationToken);
        }

        public async Task<RoleEntity> CreateAsync(RoleEntity role, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.rol (code, nombre, activo)
            VALUES (@Code, @Name, @IsActive)
            RETURNING
                id AS Id,
                code AS Code,
                nombre AS Name,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<RoleEntity>(
                sql,
                new
                {
                    role.Code,
                    role.Name,
                    role.IsActive
                },
                cancellationToken: cancellationToken);

            return created!;
        }

        public async Task<RoleEntity?> UpdateAsync(Guid id, RoleEntity role, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.rol
            SET
                code = @Code,
                nombre = @Name,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                code AS Code,
                nombre AS Name,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<RoleEntity>(
                sql,
                new
                {
                    Id = id,
                    role.Code,
                    role.Name,
                    role.IsActive
                },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.rol WHERE id = @Id;";
            var affected = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affected > 0;
        }
    }


}
