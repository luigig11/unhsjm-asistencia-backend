using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class AcademicInstanceRepository : IAcademicInstanceRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public AcademicInstanceRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<AcademicInstanceWithDetailsEntity>> GetAllAsync(
            Guid? schoolPeriodId,
            Guid? academicPlanId,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                ia.id AS Id,
                ia.id_periodo_escolar AS SchoolPeriodId,
                pe.nombre_periodo_escolar AS SchoolPeriodName,
                ia.id_detalle_plan_academico AS AcademicPlanDetailId,
                pa.codigo AS AcademicPlanCode,
                ca.nombre AS AcademicCycleName,
                pera.nombre AS AcademicPeriodName,
                ia.nombre AS Name,
                ia.fecha_inicio AS StartDate,
                ia.fecha_fin AS EndDate,
                ia.activo AS IsActive
            FROM public.instancia_academica ia
            INNER JOIN public.periodo_escolar pe ON pe.id = ia.id_periodo_escolar
            INNER JOIN public.detalle_plan_academico dpa ON dpa.id = ia.id_detalle_plan_academico
            INNER JOIN public.plan_academico pa ON pa.id = dpa.id_plan_academico
            INNER JOIN public.ciclo_academico ca ON ca.id = dpa.id_ciclo_academico
            INNER JOIN public.periodo_academico pera ON pera.id = dpa.id_periodo_academico
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();

            if (schoolPeriodId.HasValue)
            {
                sqlBuilder.Append(" AND ia.id_periodo_escolar = @SchoolPeriodId");
                parameters.Add("SchoolPeriodId", schoolPeriodId.Value);
            }

            if (academicPlanId.HasValue)
            {
                sqlBuilder.Append(" AND dpa.id_plan_academico = @AcademicPlanId");
                parameters.Add("AcademicPlanId", academicPlanId.Value);
            }

            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND ia.activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY ia.fecha_inicio DESC, ia.nombre ASC;");

            var items = await _dbContext.QueryAsync<AcademicInstanceWithDetailsEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<AcademicInstanceEntity> CreateAsync(AcademicInstanceEntity instance, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.instancia_academica (id_periodo_escolar, id_detalle_plan_academico, nombre, fecha_inicio, fecha_fin, activo)
            VALUES (@SchoolPeriodId, @AcademicPlanDetailId, @Name, @StartDate, @EndDate, @IsActive)
            RETURNING
                id AS Id,
                id_periodo_escolar AS SchoolPeriodId,
                id_detalle_plan_academico AS AcademicPlanDetailId,
                nombre AS Name,
                fecha_inicio AS StartDate,
                fecha_fin AS EndDate,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<AcademicInstanceEntity>(sql, instance, cancellationToken: cancellationToken);
            return created!;
        }

        public async Task<AcademicInstanceEntity?> UpdateAsync(Guid id, AcademicInstanceEntity instance, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.instancia_academica
            SET
                id_periodo_escolar = @SchoolPeriodId,
                id_detalle_plan_academico = @AcademicPlanDetailId,
                nombre = @Name,
                fecha_inicio = @StartDate,
                fecha_fin = @EndDate,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                id_periodo_escolar AS SchoolPeriodId,
                id_detalle_plan_academico AS AcademicPlanDetailId,
                nombre AS Name,
                fecha_inicio AS StartDate,
                fecha_fin AS EndDate,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<AcademicInstanceEntity>(
                sql,
                new
                {
                    Id = id,
                    instance.SchoolPeriodId,
                    instance.AcademicPlanDetailId,
                    instance.Name,
                    instance.StartDate,
                    instance.EndDate,
                    instance.IsActive
                },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.instancia_academica WHERE id = @Id;";
            var affectedRows = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affectedRows > 0;
        }
    }
}