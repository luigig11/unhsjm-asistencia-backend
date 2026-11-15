using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IAcademicPlanDetailRepository
    {
        Task<IReadOnlyList<AcademicPlanDetailWithNamesEntity>> GetAllAsync(
            Guid? academicPlanId,
            bool? isActive,
            CancellationToken cancellationToken = default);

        Task<AcademicPlanDetailEntity> CreateAsync(AcademicPlanDetailEntity detail, CancellationToken cancellationToken = default);

        Task<AcademicPlanDetailEntity?> UpdateAsync(Guid id, AcademicPlanDetailEntity detail, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}