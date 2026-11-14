using Dapper;
using System.Text;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Users;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public UserRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> InsertAnonUserAsync(UserEntity user, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.usuario (
                id_user,
                id_rol,
                cedula,
                nombre,
                apellido,
                genero,
                fecha_nacimiento,
                activo
            )
            VALUES (
                @AuthUserId,
                @RoleId,
                @NationalId,
                @FirstName,
                @LastName,
                @Gender::tipo_genero,
                @BirthDate,
                true
            )
            RETURNING id;
            """;

            return await _dbContext.ExecuteScalarAsync<Guid>(
                sql,
                new
                {
                    user.AuthUserId,
                    user.RoleId,
                    user.NationalId,
                    user.FirstName,
                    user.LastName,
                    user.Gender,
                    user.BirthDate
                },
                asAnonymous: true,
                cancellationToken: cancellationToken);
        }

        public async Task<UserWithDetailsEntity?> GetByAuthUserIdAsync(
            Guid authUserId,
            bool asAnonymous = false,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                u.id AS Id,
                u.id_user AS AuthUserId,
                u.id_rol AS RoleId,
                r.code AS RoleCode,
                r.nombre AS RoleName,
                u.cedula AS NationalId,
                u.nombre AS FirstName,
                u.apellido AS LastName,
                u.genero::text AS Gender,
                u.fecha_nacimiento AS BirthDate,
                u.activo AS IsActive,
                d.id AS TeacherId,
                d.id_direccion AS DepartmentId,
                d.codigo_empleado AS EmployeeCode,
                d.telefono AS Phone,
                d.tipo_contratacion::text AS ContractType,
                d.activo AS TeacherIsActive
            FROM public.usuario u
            INNER JOIN public.rol r ON r.id = u.id_rol
            LEFT JOIN public.docente d ON d.id_usuario = u.id
            WHERE u.id_user = @AuthUserId;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<UserWithDetailsEntity>(
                sql,
                new { AuthUserId = authUserId },
                asAnonymous: asAnonymous,
                overrideAuthUserId: asAnonymous ? null : authUserId.ToString(),
                cancellationToken: cancellationToken);
        }

        public async Task<(int Total, IReadOnlyList<UserWithDetailsEntity> Items)> GetPagedAsync(
            UserFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            var whereBuilder = new StringBuilder("WHERE 1 = 1");
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                whereBuilder.Append("""
                 AND (
                    u.cedula ILIKE @Search
                    OR u.nombre ILIKE @Search
                    OR u.apellido ILIKE @Search
                    OR (u.nombre || ' ' || u.apellido) ILIKE @Search
                )
                """);
                parameters.Add("Search", $"%{filter.Search.Trim()}%");
            }

            if (filter.RoleId.HasValue)
            {
                whereBuilder.Append(" AND u.id_rol = @RoleId");
                parameters.Add("RoleId", filter.RoleId.Value);
            }

            if (filter.IsActive.HasValue)
            {
                whereBuilder.Append(" AND u.activo = @IsActive");
                parameters.Add("IsActive", filter.IsActive.Value);
            }

            var page = Math.Max(1, filter.Page);
            var pageSize = Math.Clamp(filter.PageSize, 1, 100);
            var offset = (page - 1) * pageSize;

            parameters.Add("Limit", pageSize);
            parameters.Add("Offset", offset);

            var countSql = $"""
            SELECT COUNT(1)
            FROM public.usuario u
            INNER JOIN public.rol r ON r.id = u.id_rol
            {whereBuilder};
            """;

            var itemsSql = $"""
            SELECT
                u.id AS Id,
                u.id_user AS AuthUserId,
                u.id_rol AS RoleId,
                r.code AS RoleCode,
                r.nombre AS RoleName,
                u.cedula AS NationalId,
                u.nombre AS FirstName,
                u.apellido AS LastName,
                u.genero::text AS Gender,
                u.fecha_nacimiento AS BirthDate,
                u.activo AS IsActive,
                d.id AS TeacherId,
                d.id_direccion AS DepartmentId,
                d.codigo_empleado AS EmployeeCode,
                d.telefono AS Phone,
                d.tipo_contratacion::text AS ContractType,
                d.activo AS TeacherIsActive
            FROM public.usuario u
            INNER JOIN public.rol r ON r.id = u.id_rol
            LEFT JOIN public.docente d ON d.id_usuario = u.id
            {whereBuilder}
            ORDER BY u.apellido ASC, u.nombre ASC
            LIMIT @Limit OFFSET @Offset;
            """;

            var total = await _dbContext.ExecuteScalarAsync<int>(countSql, parameters, cancellationToken: cancellationToken);
            var items = await _dbContext.QueryAsync<UserWithDetailsEntity>(itemsSql, parameters, cancellationToken: cancellationToken);

            return (total, items.ToList());
        }

        public async Task<UserWithDetailsEntity?> GetByIdWithTeacherAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                u.id AS Id,
                u.id_user AS AuthUserId,
                u.id_rol AS RoleId,
                r.code AS RoleCode,
                r.nombre AS RoleName,
                u.cedula AS NationalId,
                u.nombre AS FirstName,
                u.apellido AS LastName,
                u.genero::text AS Gender,
                u.fecha_nacimiento AS BirthDate,
                u.activo AS IsActive,
                d.id AS TeacherId,
                d.id_direccion AS DepartmentId,
                d.codigo_empleado AS EmployeeCode,
                d.telefono AS Phone,
                d.tipo_contratacion::text AS ContractType,
                d.activo AS TeacherIsActive
            FROM public.usuario u
            INNER JOIN public.rol r ON r.id = u.id_rol
            LEFT JOIN public.docente d ON d.id_usuario = u.id
            WHERE u.id = @Id;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<UserWithDetailsEntity>(
                sql,
                new { Id = id },
                cancellationToken: cancellationToken);
        }

        public async Task<UserEntity?> UpdateAsync(Guid id, UserEntity user, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.usuario
            SET
                id_rol = @RoleId,
                cedula = @NationalId,
                nombre = @FirstName,
                apellido = @LastName,
                genero = @Gender::tipo_genero,
                fecha_nacimiento = @BirthDate,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                id_user AS AuthUserId,
                id_rol AS RoleId,
                cedula AS NationalId,
                nombre AS FirstName,
                apellido AS LastName,
                genero::text AS Gender,
                fecha_nacimiento AS BirthDate,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<UserEntity>(
                sql,
                new
                {
                    Id = id,
                    user.RoleId,
                    user.NationalId,
                    user.FirstName,
                    user.LastName,
                    user.Gender,
                    user.BirthDate,
                    user.IsActive
                },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.usuario WHERE id = @Id;";
            var affected = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affected > 0;
        }
    }


}
