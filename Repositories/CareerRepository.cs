using Dapper;
using System.Text;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class CareerRepository : ICareerRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public CareerRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<CareerWithDepartmentEntity>> GetAllAsync(
            string? search,
            Guid? departmentId,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                c.id AS Id,
                c.id_direccion AS DepartmentId,
                d.nombre AS DepartmentName,
                c.codigo AS Code,
                c.nombre AS Name,
                c.activo AS IsActive
            FROM public.carrera c
            INNER JOIN public.direccion d ON d.id = c.id_direccion
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(search))
            {
                sqlBuilder.Append(" AND (c.codigo ILIKE @Search OR c.nombre ILIKE @Search)");
                parameters.Add("Search", $"%{search.Trim()}%");
            }

            if (departmentId.HasValue)
            {
                sqlBuilder.Append(" AND c.id_direccion = @DepartmentId");
                parameters.Add("DepartmentId", departmentId.Value);
            }

            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND c.activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY c.nombre ASC;");

            var items = await _dbContext.QueryAsync<CareerWithDepartmentEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<CareerEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                id AS Id,
                id_direccion AS DepartmentId,
                codigo AS Code,
                nombre AS Name,
                activo AS IsActive
            FROM public.carrera
            WHERE id = @Id;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<CareerEntity>(sql, new { Id = id }, cancellationToken: cancellationToken);
        }

        public async Task<CareerEntity> CreateAsync(CareerEntity career, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.carrera (id_direccion, codigo, nombre, activo)
            VALUES (@DepartmentId, @Code, @Name, @IsActive)
            RETURNING id AS Id, id_direccion AS DepartmentId, codigo AS Code, nombre AS Name, activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<CareerEntity>(
                sql,
                new { career.DepartmentId, career.Code, career.Name, career.IsActive },
                cancellationToken: cancellationToken);

            return created!;
        }

        public async Task<CareerEntity?> UpdateAsync(Guid id, CareerEntity career, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.carrera
            SET id_direccion = @DepartmentId, codigo = @Code, nombre = @Name, activo = @IsActive
            WHERE id = @Id
            RETURNING id AS Id, id_direccion AS DepartmentId, codigo AS Code, nombre AS Name, activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<CareerEntity>(
                sql,
                new { Id = id, career.DepartmentId, career.Code, career.Name, career.IsActive },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.carrera WHERE id = @Id;";
            var affected = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affected > 0;
        }
    }
}
