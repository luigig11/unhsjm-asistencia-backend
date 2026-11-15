using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class HolidayRepository : IHolidayRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public HolidayRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<HolidayWithEventTypeEntity>> GetAllAsync(
            Guid? schoolPeriodId,
            Guid? eventTypeId,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                f.id AS Id,
                f.id_periodo_escolar AS SchoolPeriodId,
                f.id_tipo_evento AS EventTypeId,
                te.codigo AS EventTypeCode,
                te.nombre AS EventTypeName,
                f.nombre AS Name,
                f.fecha_inicio AS StartDate,
                f.fecha_fin AS EndDate,
                f.activo AS IsActive
            FROM public.feriado f
            INNER JOIN public.tipo_evento te ON te.id = f.id_tipo_evento
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();

            if (schoolPeriodId.HasValue)
            {
                sqlBuilder.Append(" AND f.id_periodo_escolar = @SchoolPeriodId");
                parameters.Add("SchoolPeriodId", schoolPeriodId.Value);
            }

            if (eventTypeId.HasValue)
            {
                sqlBuilder.Append(" AND f.id_tipo_evento = @EventTypeId");
                parameters.Add("EventTypeId", eventTypeId.Value);
            }

            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND f.activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY f.fecha_inicio ASC;");

            var items = await _dbContext.QueryAsync<HolidayWithEventTypeEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<HolidayEntity> CreateAsync(HolidayEntity holiday, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.feriado (id_periodo_escolar, id_tipo_evento, nombre, fecha_inicio, fecha_fin, activo)
            VALUES (@SchoolPeriodId, @EventTypeId, @Name, @StartDate, @EndDate, @IsActive)
            RETURNING
                id AS Id,
                id_periodo_escolar AS SchoolPeriodId,
                id_tipo_evento AS EventTypeId,
                nombre AS Name,
                fecha_inicio AS StartDate,
                fecha_fin AS EndDate,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<HolidayEntity>(sql, holiday, cancellationToken: cancellationToken);
            return created!;
        }

        public async Task<HolidayEntity?> UpdateAsync(Guid id, HolidayEntity holiday, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.feriado
            SET
                id_periodo_escolar = @SchoolPeriodId,
                id_tipo_evento = @EventTypeId,
                nombre = @Name,
                fecha_inicio = @StartDate,
                fecha_fin = @EndDate,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                id_periodo_escolar AS SchoolPeriodId,
                id_tipo_evento AS EventTypeId,
                nombre AS Name,
                fecha_inicio AS StartDate,
                fecha_fin AS EndDate,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<HolidayEntity>(
                sql,
                new
                {
                    Id = id,
                    holiday.SchoolPeriodId,
                    holiday.EventTypeId,
                    holiday.Name,
                    holiday.StartDate,
                    holiday.EndDate,
                    holiday.IsActive
                },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.feriado WHERE id = @Id;";
            var affectedRows = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affectedRows > 0;
        }
    }
}