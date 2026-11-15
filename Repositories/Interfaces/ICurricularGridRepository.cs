using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface ICurricularGridRepository
    {
        Task<IReadOnlyList<CurricularGridItemWithSubjectEntity>> GetByPlanIdAsync(
            Guid academicPlanId,
            CancellationToken cancellationToken = default);

        Task<CurricularGridItemEntity> CreateAsync(CurricularGridItemEntity item, CancellationToken cancellationToken = default);

        Task<CurricularGridItemEntity?> UpdateAsync(Guid id, CurricularGridItemEntity item, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }

}