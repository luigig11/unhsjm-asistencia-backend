using System.Text;
using Dapper;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class AcademicPlanRepository : IAcademicPlanRepository
    {
        private const string DetailedSelectSql = """
        SELECT
            pa.id AS Id,
            pa.id_centro_universitario AS UniversityCenterId,
            cu.nombre AS UniversityCenterName,
            c.id_direccion AS DepartmentId,
            d.nombre AS DepartmentName,
            pa.id_carrera AS CareerId,
            c.nombre AS CareerName,
            pa.id_tipo_periodo_academico AS AcademicPeriodTypeId,
            tpa.nombre AS AcademicPeriodTypeName,
            pa.id_modalidad AS ModalityId,
            m.nombre AS ModalityName,
            pa.codigo AS Code,
            pa.nombre AS Name,
            pa.cantidad_ciclo_academico AS AcademicCycleCount,
            pa.cantidad_periodo_academico AS AcademicPeriodCount,
            pa.activo_desde AS ActiveFrom,
            pa.activo_hasta AS ActiveUntil,
            pa.activo AS IsActive
        FROM public.plan_academico pa
        INNER JOIN public.centro_universitario cu ON cu.id = pa.id_centro_universitario
        INNER JOIN public.carrera c ON c.id = pa.id_carrera
        INNER JOIN public.direccion d ON d.id = c.id_direccion
        INNER JOIN public.tipo_periodo_academico tpa ON tpa.id = pa.id_tipo_periodo_academico
        INNER JOIN public.modalidad m ON m.id = pa.id_modalidad
        """;

        private readonly IDapperRlsContext _dbContext;

        public AcademicPlanRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<AcademicPlanWithDetailsEntity>> GetAllAsync(
            Guid? departmentId,
            Guid? careerId,
            Guid? modalityId,
            Guid? universityCenterId,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new StringBuilder(DetailedSelectSql);
            sqlBuilder.Append(" WHERE 1 = 1");

            var parameters = new DynamicParameters();

            if (departmentId.HasValue)
            {
                sqlBuilder.Append(" AND c.id_direccion = @DepartmentId");
                parameters.Add("DepartmentId", departmentId.Value);
            }

            if (careerId.HasValue)
            {
                sqlBuilder.Append(" AND pa.id_carrera = @CareerId");
                parameters.Add("CareerId", careerId.Value);
            }

            if (modalityId.HasValue)
            {
                sqlBuilder.Append(" AND pa.id_modalidad = @ModalityId");
                parameters.Add("ModalityId", modalityId.Value);
            }

            if (universityCenterId.HasValue)
            {
                sqlBuilder.Append(" AND pa.id_centro_universitario = @UniversityCenterId");
                parameters.Add("UniversityCenterId", universityCenterId.Value);
            }

            if (isActive.HasValue)
            {
                sqlBuilder.Append(" AND pa.activo = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            sqlBuilder.Append(" ORDER BY pa.nombre ASC;");

            var items = await _dbContext.QueryAsync<AcademicPlanWithDetailsEntity>(sqlBuilder.ToString(), parameters, cancellationToken: cancellationToken);
            return items.ToList();
        }

        public async Task<AcademicPlanEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                id AS Id,
                id_centro_universitario AS UniversityCenterId,
                id_carrera AS CareerId,
                id_tipo_periodo_academico AS AcademicPeriodTypeId,
                id_modalidad AS ModalityId,
                codigo AS Code,
                nombre AS Name,
                cantidad_ciclo_academico AS AcademicCycleCount,
                cantidad_periodo_academico AS AcademicPeriodCount,
                activo_desde AS ActiveFrom,
                activo_hasta AS ActiveUntil,
                activo AS IsActive
            FROM public.plan_academico
            WHERE id = @Id;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<AcademicPlanEntity>(sql, new { Id = id }, cancellationToken: cancellationToken);
        }

        public async Task<AcademicPlanWithDetailsEntity?> GetDetailedByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var sql = $"{DetailedSelectSql} WHERE pa.id = @Id;";
            return await _dbContext.QueryFirstOrDefaultAsync<AcademicPlanWithDetailsEntity>(sql, new { Id = id }, cancellationToken: cancellationToken);
        }

        public async Task<AcademicPlanEntity> CreateAsync(AcademicPlanEntity plan, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.plan_academico (
                id_centro_universitario,
                id_carrera,
                id_tipo_periodo_academico,
                id_modalidad,
                codigo,
                nombre,
                cantidad_ciclo_academico,
                cantidad_periodo_academico,
                activo_desde,
                activo_hasta,
                activo)
            VALUES (
                @UniversityCenterId,
                @CareerId,
                @AcademicPeriodTypeId,
                @ModalityId,
                @Code,
                @Name,
                @AcademicCycleCount,
                @AcademicPeriodCount,
                @ActiveFrom,
                @ActiveUntil,
                @IsActive)
            RETURNING
                id AS Id,
                id_centro_universitario AS UniversityCenterId,
                id_carrera AS CareerId,
                id_tipo_periodo_academico AS AcademicPeriodTypeId,
                id_modalidad AS ModalityId,
                codigo AS Code,
                nombre AS Name,
                cantidad_ciclo_academico AS AcademicCycleCount,
                cantidad_periodo_academico AS AcademicPeriodCount,
                activo_desde AS ActiveFrom,
                activo_hasta AS ActiveUntil,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<AcademicPlanEntity>(sql, plan, cancellationToken: cancellationToken);
            return created!;
        }

        public async Task<AcademicPlanEntity?> UpdateAsync(Guid id, AcademicPlanEntity plan, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.plan_academico
            SET
                id_centro_universitario = @UniversityCenterId,
                id_carrera = @CareerId,
                id_tipo_periodo_academico = @AcademicPeriodTypeId,
                id_modalidad = @ModalityId,
                codigo = @Code,
                nombre = @Name,
                cantidad_ciclo_academico = @AcademicCycleCount,
                cantidad_periodo_academico = @AcademicPeriodCount,
                activo_desde = @ActiveFrom,
                activo_hasta = @ActiveUntil,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                id_centro_universitario AS UniversityCenterId,
                id_carrera AS CareerId,
                id_tipo_periodo_academico AS AcademicPeriodTypeId,
                id_modalidad AS ModalityId,
                codigo AS Code,
                nombre AS Name,
                cantidad_ciclo_academico AS AcademicCycleCount,
                cantidad_periodo_academico AS AcademicPeriodCount,
                activo_desde AS ActiveFrom,
                activo_hasta AS ActiveUntil,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<AcademicPlanEntity>(
                sql,
                new
                {
                    Id = id,
                    plan.UniversityCenterId,
                    plan.CareerId,
                    plan.AcademicPeriodTypeId,
                    plan.ModalityId,
                    plan.Code,
                    plan.Name,
                    plan.AcademicCycleCount,
                    plan.AcademicPeriodCount,
                    plan.ActiveFrom,
                    plan.ActiveUntil,
                    plan.IsActive
                },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.plan_academico WHERE id = @Id;";
            var affectedRows = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affectedRows > 0;
        }
    }
}