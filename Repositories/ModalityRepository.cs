using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace plantilla.Repositories
{
    public sealed class ModalityRepository : IModalityRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public ModalityRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<ModalityEntity>> GetAllAsync(
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
            FROM public.modalidad
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

            var items = await _dbContext.QueryAsync<ModalityEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<ModalityEntity> CreateAsync(ModalityEntity modality, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.modalidad (codigo, nombre, activo)
            VALUES (@Code, @Name, @IsActive)
            RETURNING id AS Id, codigo AS Code, nombre AS Name, activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<ModalityEntity>(
                sql,
                new { modality.Code, modality.Name, modality.IsActive },
                cancellationToken: cancellationToken);

            return created!;
        }

        public async Task<ModalityEntity?> UpdateAsync(Guid id, ModalityEntity modality, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.modalidad
            SET codigo = @Code, nombre = @Name, activo = @IsActive
            WHERE id = @Id
            RETURNING id AS Id, codigo AS Code, nombre AS Name, activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<ModalityEntity>(
                sql,
                new { Id = id, modality.Code, modality.Name, modality.IsActive },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.modalidad WHERE id = @Id;";
            var affected = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affected > 0;
        }
    }
}

