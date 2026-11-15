using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IModalityRepository
    {
        Task<IReadOnlyList<ModalityEntity>> GetAllAsync(string? search, bool? isActive, CancellationToken cancellationToken = default);
        Task<ModalityEntity> CreateAsync(ModalityEntity modality, CancellationToken cancellationToken = default);
        Task<ModalityEntity?> UpdateAsync(Guid id, ModalityEntity modality, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

