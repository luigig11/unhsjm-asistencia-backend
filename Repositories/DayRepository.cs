using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class DayRepository : IDayRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public DayRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<DayEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                id AS Id,
                nombre AS Name
            FROM public.dia
            ORDER BY id ASC;
            """;

            var items = await _dbContext.QueryAsync<DayEntity>(sql, cancellationToken: cancellationToken);
            return items.ToList();
        }
    }
}