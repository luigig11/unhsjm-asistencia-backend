using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class AcademicPlanDetailRepository : IAcademicPlanDetailRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public AcademicPlanDetailRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<AcademicPlanDetailWithNamesEntity>> GetAllAsync(
            Guid? academicPlanId,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder("""
            SELECT
                dpa.id AS Id,
                dpa.id_plan_academico AS AcademicPlanId,
                dpa.id_ciclo_academico AS AcademicCycleId,
                ca.codigo AS AcademicCycleCode,
                ca.nombre AS AcademicCycleName,
                ca.orden AS AcademicCycleOrder,
                dpa.id_periodo_academico AS AcademicPeriodId,
                pa.codigo AS AcademicPeriodCode,
                pa.nombre AS AcademicPeriodName,
                pa.orden AS AcademicPeriodOrder,
                dpa.duracion_semanas AS DurationWeeks,
                dpa.duracion_meses AS DurationMonths,
                dpa.id_detalle_predecesor AS PredecessorDetailId,
                dpa.activo AS IsActive
            FROM public.detalle_plan_academico dpa
            INNER JOIN public.ciclo_academico ca ON ca.id = dpa.id_ciclo_academico
            INNER JOIN public.periodo_academico pa ON pa.id = dpa.id_periodo_academico
            WHERE 1 = 1
            """);

            var parameters = new DynamicParameters();

            if (academicPlanId.HasValue)
            {
                sqlBuilder.Append(" AND dpa.id_plan_academico = @AcademicPlanId");
                parameters.Add("AcademicPlanId", academicPlanId.Value);
            }

            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND dpa.activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY ca.orden ASC, pa.orden ASC;");

            var items = await _dbContext.QueryAsync<AcademicPlanDetailWithNamesEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<AcademicPlanDetailEntity> CreateAsync(AcademicPlanDetailEntity detail, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.detalle_plan_academico (
                id_plan_academico,
                id_ciclo_academico,
                id_periodo_academico,
                duracion_semanas,
                duracion_meses,
                id_detalle_predecesor,
                activo)
            VALUES (
                @AcademicPlanId,
                @AcademicCycleId,
                @AcademicPeriodId,
                @DurationWeeks,
                @DurationMonths,
                @PredecessorDetailId,
                @IsActive)
            RETURNING
                id AS Id,
                id_plan_academico AS AcademicPlanId,
                id_ciclo_academico AS AcademicCycleId,
                id_periodo_academico AS AcademicPeriodId,
                duracion_semanas AS DurationWeeks,
                duracion_meses AS DurationMonths,
                id_detalle_predecesor AS PredecessorDetailId,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<AcademicPlanDetailEntity>(sql, detail, cancellationToken: cancellationToken);
            return created!;
        }

        public async Task<AcademicPlanDetailEntity?> UpdateAsync(Guid id, AcademicPlanDetailEntity detail, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.detalle_plan_academico
            SET
                id_plan_academico = @AcademicPlanId,
                id_ciclo_academico = @AcademicCycleId,
                id_periodo_academico = @AcademicPeriodId,
                duracion_semanas = @DurationWeeks,
                duracion_meses = @DurationMonths,
                id_detalle_predecesor = @PredecessorDetailId,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                id_plan_academico AS AcademicPlanId,
                id_ciclo_academico AS AcademicCycleId,
                id_periodo_academico AS AcademicPeriodId,
                duracion_semanas AS DurationWeeks,
                duracion_meses AS DurationMonths,
                id_detalle_predecesor AS PredecessorDetailId,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<AcademicPlanDetailEntity>(
                sql,
                new
                {
                    Id = id,
                    detail.AcademicPlanId,
                    detail.AcademicCycleId,
                    detail.AcademicPeriodId,
                    detail.DurationWeeks,
                    detail.DurationMonths,
                    detail.PredecessorDetailId,
                    detail.IsActive
                },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.detalle_plan_academico WHERE id = @Id;";
            var affectedRows = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affectedRows > 0;
        }
    }
}