using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class SchoolPeriodRepository : ISchoolPeriodRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public SchoolPeriodRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<SchoolPeriodEntity>> GetAllAsync(bool? isActive, CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                id AS Id,
                nombre_periodo_escolar AS Name,
                fecha_inicio AS StartDate,
                fecha_fin AS EndDate,
                activo AS IsActive
            FROM public.periodo_escolar
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();
            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY fecha_inicio DESC;");

            var items = await _dbContext.QueryAsync<SchoolPeriodEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<SchoolPeriodEntity> CreateAsync(SchoolPeriodEntity period, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.periodo_escolar (nombre_periodo_escolar, fecha_inicio, fecha_fin, activo)
            VALUES (@Name, @StartDate, @EndDate, @IsActive)
            RETURNING
                id AS Id,
                nombre_periodo_escolar AS Name,
                fecha_inicio AS StartDate,
                fecha_fin AS EndDate,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<SchoolPeriodEntity>(sql, period, cancellationToken: cancellationToken);
            return created!;
        }

        public async Task<SchoolPeriodEntity?> UpdateAsync(Guid id, SchoolPeriodEntity period, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.periodo_escolar
            SET
                nombre_periodo_escolar = @Name,
                fecha_inicio = @StartDate,
                fecha_fin = @EndDate,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                nombre_periodo_escolar AS Name,
                fecha_inicio AS StartDate,
                fecha_fin AS EndDate,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<SchoolPeriodEntity>(
                sql,
                new { Id = id, period.Name, period.StartDate, period.EndDate, period.IsActive },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.periodo_escolar WHERE id = @Id;";
            var affectedRows = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affectedRows > 0;
        }
    }
}