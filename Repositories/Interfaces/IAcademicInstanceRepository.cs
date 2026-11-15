using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IAcademicInstanceRepository
    {
        Task<IReadOnlyList<AcademicInstanceWithDetailsEntity>> GetAllAsync(
            Guid? schoolPeriodId,
            Guid? academicPlanId,
            bool? isActive,
            CancellationToken cancellationToken = default);

        Task<AcademicInstanceEntity> CreateAsync(AcademicInstanceEntity instance, CancellationToken cancellationToken = default);
        Task<AcademicInstanceEntity?> UpdateAsync(Guid id, AcademicInstanceEntity instance, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}