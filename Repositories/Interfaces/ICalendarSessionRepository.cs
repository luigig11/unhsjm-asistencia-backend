using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface ICalendarSessionRepository
    {
        Task<IReadOnlyList<CalendarSessionWithDetailsEntity>> GetAllAsync(
            Guid? groupSubjectId,
            DateTimeOffset? dayStartUtc,
            DateTimeOffset? dayEndUtc,
            bool? isActive,
            CancellationToken cancellationToken = default);

        Task<CalendarSessionEntity> CreateAsync(CalendarSessionEntity session, CancellationToken cancellationToken = default);

        Task<CalendarSessionEntity?> TryInsertIfNotExistsAsync(
            CalendarSessionEntity session,
            CancellationToken cancellationToken = default);

        Task<CalendarSessionEntity?> UpdateAsync(
            Guid id,
            Guid classroomId,
            DateTimeOffset startDate,
            DateTimeOffset endDate,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}