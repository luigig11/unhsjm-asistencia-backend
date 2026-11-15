using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class AcademicPeriodTypeRepository : IAcademicPeriodTypeRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public AcademicPeriodTypeRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<AcademicPeriodTypeEntity>> GetAllAsync(bool? isActive, CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                id AS Id,
                codigo AS Code,
                nombre AS Name,
                duracion_meses_minimo AS MinMonthsDuration,
                duracion_meses_maximo AS MaxMonthsDuration,
                duracion_horas_minimo AS MinHoursDuration,
                duracion_horas_maximo AS MaxHoursDuration,
                activo AS IsActive
            FROM public.tipo_periodo_academico
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();
            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY nombre ASC;");

            var items = await _dbContext.QueryAsync<AcademicPeriodTypeEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<AcademicPeriodTypeEntity> CreateAsync(AcademicPeriodTypeEntity periodType, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.tipo_periodo_academico (
                codigo, nombre, duracion_meses_minimo, duracion_meses_maximo,
                duracion_horas_minimo, duracion_horas_maximo, activo
            )
            VALUES (
                @Code, @Name, @MinMonthsDuration, @MaxMonthsDuration,
                @MinHoursDuration, @MaxHoursDuration, @IsActive
            )
            RETURNING
                id AS Id,
                codigo AS Code,
                nombre AS Name,
                duracion_meses_minimo AS MinMonthsDuration,
                duracion_meses_maximo AS MaxMonthsDuration,
                duracion_horas_minimo AS MinHoursDuration,
                duracion_horas_maximo AS MaxHoursDuration,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<AcademicPeriodTypeEntity>(
                sql,
                new
                {
                    periodType.Code,
                    periodType.Name,
                    periodType.MinMonthsDuration,
                    periodType.MaxMonthsDuration,
                    periodType.MinHoursDuration,
                    periodType.MaxHoursDuration,
                    periodType.IsActive
                },
                cancellationToken: cancellationToken);

            return created!;
        }

        public async Task<AcademicPeriodTypeEntity?> UpdateAsync(Guid id, AcademicPeriodTypeEntity periodType, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.tipo_periodo_academico
            SET
                codigo = @Code,
                nombre = @Name,
                duracion_meses_minimo = @MinMonthsDuration,
                duracion_meses_maximo = @MaxMonthsDuration,
                duracion_horas_minimo = @MinHoursDuration,
                duracion_horas_maximo = @MaxHoursDuration,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                codigo AS Code,
                nombre AS Name,
                duracion_meses_minimo AS MinMonthsDuration,
                duracion_meses_maximo AS MaxMonthsDuration,
                duracion_horas_minimo AS MinHoursDuration,
                duracion_horas_maximo AS MaxHoursDuration,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<AcademicPeriodTypeEntity>(
                sql,
                new
                {
                    Id = id,
                    periodType.Code,
                    periodType.Name,
                    periodType.MinMonthsDuration,
                    periodType.MaxMonthsDuration,
                    periodType.MinHoursDuration,
                    periodType.MaxHoursDuration,
                    periodType.IsActive
                },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.tipo_periodo_academico WHERE id = @Id;";
            var affected = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affected > 0;
        }
    }
}

