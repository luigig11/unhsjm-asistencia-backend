using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class CurricularGridRepository : ICurricularGridRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public CurricularGridRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<CurricularGridItemWithSubjectEntity>> GetByPlanIdAsync(
            Guid academicPlanId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                mc.id AS Id,
                mc.id_detalle_plan_academico AS AcademicPlanDetailId,
                mc.id_materia AS SubjectId,
                m.codigo AS SubjectCode,
                m.nombre AS SubjectName,
                mc.encuentros_totales AS TotalMeetings,
                mc.encuentros_por_semana AS MeetingsPerWeek,
                mc.cantidad_horas_materia AS SubjectHours,
                mc.creditos AS Credits,
                mc.obligatoria AS IsMandatory,
                mc.activo AS IsActive
            FROM public.malla_curricular mc
            INNER JOIN public.detalle_plan_academico dpa ON dpa.id = mc.id_detalle_plan_academico
            INNER JOIN public.materia m ON m.id = mc.id_materia
            WHERE dpa.id_plan_academico = @AcademicPlanId
            ORDER BY m.nombre ASC;
            """;

            var items = await _dbContext.QueryAsync<CurricularGridItemWithSubjectEntity>(
                sql,
                new { AcademicPlanId = academicPlanId },
                cancellationToken: cancellationToken);

            return items.ToList();
        }

        public async Task<CurricularGridItemEntity> CreateAsync(CurricularGridItemEntity item, CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.malla_curricular (
                id_detalle_plan_academico,
                id_materia,
                encuentros_totales,
                encuentros_por_semana,
                cantidad_horas_materia,
                creditos,
                obligatoria,
                activo)
            VALUES (
                @AcademicPlanDetailId,
                @SubjectId,
                @TotalMeetings,
                @MeetingsPerWeek,
                @SubjectHours,
                @Credits,
                @IsMandatory,
                @IsActive)
            RETURNING
                id AS Id,
                id_detalle_plan_academico AS AcademicPlanDetailId,
                id_materia AS SubjectId,
                encuentros_totales AS TotalMeetings,
                encuentros_por_semana AS MeetingsPerWeek,
                cantidad_horas_materia AS SubjectHours,
                creditos AS Credits,
                obligatoria AS IsMandatory,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<CurricularGridItemEntity>(sql, item, cancellationToken: cancellationToken);
            return created!;
        }

        public async Task<CurricularGridItemEntity?> UpdateAsync(Guid id, CurricularGridItemEntity item, CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE public.malla_curricular
            SET
                id_detalle_plan_academico = @AcademicPlanDetailId,
                id_materia = @SubjectId,
                encuentros_totales = @TotalMeetings,
                encuentros_por_semana = @MeetingsPerWeek,
                cantidad_horas_materia = @SubjectHours,
                creditos = @Credits,
                obligatoria = @IsMandatory,
                activo = @IsActive
            WHERE id = @Id
            RETURNING
                id AS Id,
                id_detalle_plan_academico AS AcademicPlanDetailId,
                id_materia AS SubjectId,
                encuentros_totales AS TotalMeetings,
                encuentros_por_semana AS MeetingsPerWeek,
                cantidad_horas_materia AS SubjectHours,
                creditos AS Credits,
                obligatoria AS IsMandatory,
                activo AS IsActive;
            """;

            return await _dbContext.QueryFirstOrDefaultAsync<CurricularGridItemEntity>(
                sql,
                new
                {
                    Id = id,
                    item.AcademicPlanDetailId,
                    item.SubjectId,
                    item.TotalMeetings,
                    item.MeetingsPerWeek,
                    item.SubjectHours,
                    item.Credits,
                    item.IsMandatory,
                    item.IsActive
                },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.malla_curricular WHERE id = @Id;";
            var affectedRows = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affectedRows > 0;
        }
    }
}