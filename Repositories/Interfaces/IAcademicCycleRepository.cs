using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IAcademicCycleRepository
    {
        Task<IReadOnlyList<AcademicCycleEntity>> GetAllAsync(bool? isActive, CancellationToken cancellationToken = default);
        Task<AcademicCycleEntity> CreateAsync(AcademicCycleEntity cycle, CancellationToken cancellationToken = default);
        Task<AcademicCycleEntity?> UpdateAsync(Guid id, AcademicCycleEntity cycle, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

