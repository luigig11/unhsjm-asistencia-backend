using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IHolidayRepository
    {
        Task<IReadOnlyList<HolidayWithEventTypeEntity>> GetAllAsync(
            Guid? schoolPeriodId,
            Guid? eventTypeId,
            bool? isActive,
            CancellationToken cancellationToken = default);

        Task<HolidayEntity> CreateAsync(HolidayEntity holiday, CancellationToken cancellationToken = default);
        Task<HolidayEntity?> UpdateAsync(Guid id, HolidayEntity holiday, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}