using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface ISubjectPrerequisiteRepository
    {
        Task<IReadOnlyList<SubjectPrerequisiteWithSubjectEntity>> GetByPlanIdAsync(
            Guid academicPlanId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Guid>> GetPrerequisiteGridItemIdsAsync(
            Guid curricularGridItemId,
            CancellationToken cancellationToken = default);

        Task ReplacePrerequisitesAsync(
            Guid curricularGridItemId,
            IReadOnlyList<Guid> prerequisiteGridItemIds,
            CancellationToken cancellationToken = default);

        Task<SubjectPrerequisiteEntity> CreateAsync(
            SubjectPrerequisiteEntity prerequisite,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}