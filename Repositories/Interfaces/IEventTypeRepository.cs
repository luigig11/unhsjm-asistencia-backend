using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IEventTypeRepository
    {
        Task<IReadOnlyList<EventTypeEntity>> GetAllAsync(bool? isActive, CancellationToken cancellationToken = default);
        Task<EventTypeEntity> CreateAsync(EventTypeEntity eventType, CancellationToken cancellationToken = default);
        Task<EventTypeEntity?> UpdateAsync(Guid id, EventTypeEntity eventType, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}