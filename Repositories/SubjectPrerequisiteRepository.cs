using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Repositories
{
    public sealed class SubjectPrerequisiteRepository : ISubjectPrerequisiteRepository
    {
        private readonly IDapperRlsContext _dbContext;

        public SubjectPrerequisiteRepository(IDapperRlsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<SubjectPrerequisiteWithSubjectEntity>> GetByPlanIdAsync(
            Guid academicPlanId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT
                mp.id AS Id,
                mp.id_malla_curricular_materia AS CurricularGridItemId,
                mp.id_malla_curricular_materia_prerequisito AS PrerequisiteCurricularGridItemId,
                mc_pre.id_materia AS PrerequisiteSubjectId,
                m_pre.codigo AS PrerequisiteSubjectCode,
                m_pre.nombre AS PrerequisiteSubjectName,
                mp.activo AS IsActive
            FROM public.materia_prerequisito mp
            INNER JOIN public.malla_curricular mc_cur ON mc_cur.id = mp.id_malla_curricular_materia
            INNER JOIN public.detalle_plan_academico dpa ON dpa.id = mc_cur.id_detalle_plan_academico
            INNER JOIN public.malla_curricular mc_pre ON mc_pre.id = mp.id_malla_curricular_materia_prerequisito
            INNER JOIN public.materia m_pre ON m_pre.id = mc_pre.id_materia
            WHERE dpa.id_plan_academico = @AcademicPlanId
            ORDER BY m_pre.nombre ASC;
            """;

            var items = await _dbContext.QueryAsync<SubjectPrerequisiteWithSubjectEntity>(
                sql,
                new { AcademicPlanId = academicPlanId },
                cancellationToken: cancellationToken);

            return items.ToList();
        }

        public async Task<IReadOnlyList<Guid>> GetPrerequisiteGridItemIdsAsync(
            Guid curricularGridItemId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            SELECT id_malla_curricular_materia_prerequisito
            FROM public.materia_prerequisito
            WHERE id_malla_curricular_materia = @CurricularGridItemId AND activo = TRUE;
            """;

            var ids = await _dbContext.QueryAsync<Guid>(
                sql,
                new { CurricularGridItemId = curricularGridItemId },
                cancellationToken: cancellationToken);

            return ids.ToList();
        }

        public async Task ReplacePrerequisitesAsync(
            Guid curricularGridItemId,
            IReadOnlyList<Guid> prerequisiteGridItemIds,
            CancellationToken cancellationToken = default)
        {
            const string deleteSql = """
            DELETE FROM public.materia_prerequisito
            WHERE id_malla_curricular_materia = @CurricularGridItemId;
            """;

            await _dbContext.ExecuteAsync(
                deleteSql,
                new { CurricularGridItemId = curricularGridItemId },
                cancellationToken: cancellationToken);

            if (prerequisiteGridItemIds.Count == 0)
            {
                return;
            }

            const string insertSql = """
            INSERT INTO public.materia_prerequisito (
                id_malla_curricular_materia,
                id_malla_curricular_materia_prerequisito,
                activo)
            VALUES (
                @CurricularGridItemId,
                @PrerequisiteCurricularGridItemId,
                TRUE);
            """;

            var rows = prerequisiteGridItemIds.Select(prereqId => new
            {
                CurricularGridItemId = curricularGridItemId,
                PrerequisiteCurricularGridItemId = prereqId
            });

            await _dbContext.ExecuteAsync(insertSql, rows, cancellationToken: cancellationToken);
        }

        public async Task<SubjectPrerequisiteEntity> CreateAsync(
            SubjectPrerequisiteEntity prerequisite,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO public.materia_prerequisito (
                id_malla_curricular_materia,
                id_malla_curricular_materia_prerequisito,
                activo)
            VALUES (
                @CurricularGridItemId,
                @PrerequisiteCurricularGridItemId,
                @IsActive)
            RETURNING
                id AS Id,
                id_malla_curricular_materia AS CurricularGridItemId,
                id_malla_curricular_materia_prerequisito AS PrerequisiteCurricularGridItemId,
                activo AS IsActive;
            """;

            var created = await _dbContext.QueryFirstOrDefaultAsync<SubjectPrerequisiteEntity>(sql, prerequisite, cancellationToken: cancellationToken);
            return created!;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = "DELETE FROM public.materia_prerequisito WHERE id = @Id;";
            var affectedRows = await _dbContext.ExecuteAsync(sql, new { Id = id }, cancellationToken: cancellationToken);
            return affectedRows > 0;
        }
    }

}