using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class EventTypeRepository : IEventTypeRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public EventTypeRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<EventTypeEntity>> GetAllAsync(bool? isActive, CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                id AS Id,
                codigo AS Code,
                nombre AS Name,
                activo AS IsActive
            FROM public.tipo_evento
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();
            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY nombre ASC;");

            var items = await _dbContext.QueryAsync<EventTypeEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<EventTypeEntity> CreateAsync(EventTypeEntity eventType, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.tipo_evento (codigo, nombre, activo)
            VALUES (@Code, @Name, @IsActive)
            RETURNING id AS Id, codigo AS Code, nombre AS Name, activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<EventTypeEntity>(
                sql,
                new { eventType.Code, eventType.Name, eventType.IsActive },
                cancellationToken: cancellationToken);

            return created!;
        }

        public async Task<EventTypeEntity?> UpdateAsync(Guid id, EventTypeEntity eventType, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.tipo_evento
            SET codigo = @Code, nombre = @Name, activo = @IsActive
            WHERE id = @Id
            RETURNING id AS Id, codigo AS Code, nombre AS Name, activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<EventTypeEntity>(
                sql,
                new { Id = id, eventType.Code, eventType.Name, eventType.IsActive },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.tipo_evento WHERE id = @Id;";
            var affected = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affected > 0;
        }
    }
}