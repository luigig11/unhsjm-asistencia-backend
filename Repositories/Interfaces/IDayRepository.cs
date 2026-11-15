using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IDayRepository
    {
        Task<IReadOnlyList<DayEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}