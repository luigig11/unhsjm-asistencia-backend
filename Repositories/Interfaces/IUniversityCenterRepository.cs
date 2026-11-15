using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IUniversityCenterRepository
    {
        Task<IReadOnlyList<UniversityCenterEntity>> GetAllAsync(string? search, bool? isActive, CancellationToken cancellationToken = default);
        Task<UniversityCenterEntity> CreateAsync(UniversityCenterEntity center, CancellationToken cancellationToken = default);
        Task<UniversityCenterEntity?> UpdateAsync(Guid id, UniversityCenterEntity center, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}