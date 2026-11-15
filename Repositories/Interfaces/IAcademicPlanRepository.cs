using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IAcademicPlanRepository
    {
        Task<IReadOnlyList<AcademicPlanWithDetailsEntity>> GetAllAsync(
            Guid? departmentId,
            Guid? careerId,
            Guid? modalityId,
            Guid? universityCenterId,
            bool? isActive,
            CancellationToken cancellationToken = default);

        Task<AcademicPlanEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<AcademicPlanWithDetailsEntity?> GetDetailedByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<AcademicPlanEntity> CreateAsync(AcademicPlanEntity plan, CancellationToken cancellationToken = default);

        Task<AcademicPlanEntity?> UpdateAsync(Guid id, AcademicPlanEntity plan, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }

}