using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IAcademicPeriodTypeRepository
    {
        Task<IReadOnlyList<AcademicPeriodTypeEntity>> GetAllAsync(bool? isActive, CancellationToken cancellationToken = default);
        Task<AcademicPeriodTypeEntity> CreateAsync(AcademicPeriodTypeEntity periodType, CancellationToken cancellationToken = default);
        Task<AcademicPeriodTypeEntity?> UpdateAsync(Guid id, AcademicPeriodTypeEntity periodType, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

