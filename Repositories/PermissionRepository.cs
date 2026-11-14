using Dapper;
using System.Text;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class PermissionRepository : IPermissionRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public PermissionRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<PermissionEntity>> GetAllAsync(
            Guid? roleId = null,
            string? entityName = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                id AS Id,
                id_rol AS RoleId,
                entidad AS EntityName,
                operaciones_permitidas::text[] AS AllowedOperations,
                fecha_creacion AS CreatedAt,
                fecha_actualizacion AS UpdatedAt,
                activo AS IsActive
            FROM public.permiso
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();

            if (roleId.HasValue)
            {
                sqlBuilder.Append(" AND id_rol = @RoleId");
                parameters.Add("RoleId", roleId.Value);
            }

            if (!string.IsNullOrWhiteSpace(entityName))
            {
                sqlBuilder.Append(" AND lower(entidad) = lower(@EntityName)");
                parameters.Add("EntityName", entityName.Trim());
            }

            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY entidad ASC;");

            var items = await _dbContext.QueryAsync<PermissionEntity>(
                sqlBuilder.ToString(),
                parameters,
                cancellationToken: cancellationToken);

            return items.ToList();
        }

        public async Task<PermissionEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                id AS Id,
                id_rol AS RoleId,
                entidad AS EntityName,
                operaciones_permitidas::text[] AS AllowedOperations,
                fecha_creacion AS CreatedAt,
                fecha_actualizacion AS UpdatedAt,
                activo AS IsActive
            FROM public.permiso
            WHERE id = @Id;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<PermissionEntity>(
                sql,
                new { Id = id },
                cancellationToken: cancellationToken);
        }

        public async Task<PermissionEntity> CreateAsync(PermissionEntity permission, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.permiso (
                id_rol,
                entidad,
                operaciones_permitidas,
                activo
            )
            VALUES (
                @RoleId,
                @EntityName,
                @AllowedOperations::tipo_operacion[],
                @IsActive
            )
            RETURNING
                id AS Id,
                id_rol AS RoleId,
                entidad AS EntityName,
                operaciones_permitidas::text[] AS AllowedOperations,
                fecha_creacion AS CreatedAt,
                fecha_actualizacion AS UpdatedAt,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<PermissionEntity>(
                sql,
                new
                {
                    permission.RoleId,
                    permission.EntityName,
                    permission.AllowedOperations,
                    permission.IsActive
                },
                cancellationToken: cancellationToken);

            return created!;
        }

        public async Task<PermissionEntity?> UpdateAsync(Guid id, PermissionEntity permission, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.permiso
            SET
                operaciones_permitidas = @AllowedOperations::tipo_operacion[],
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                id_rol AS RoleId,
                entidad AS EntityName,
                operaciones_permitidas::text[] AS AllowedOperations,
                fecha_creacion AS CreatedAt,
                fecha_actualizacion AS UpdatedAt,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<PermissionEntity>(
                sql,
                new
                {
                    Id = id,
                    permission.AllowedOperations,
                    permission.IsActive
                },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.permiso WHERE id = @Id;";
            var affected = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affected > 0;
        }
    }


}
