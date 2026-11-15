using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IWeeklyScheduleRepository
    {
        Task<IReadOnlyList<WeeklyScheduleWithDetailsEntity>> GetAllAsync(
            Guid? groupSubjectId,
            bool? isActive,
            CancellationToken cancellationToken = default);

        Task<WeeklyScheduleEntity> CreateAsync(WeeklyScheduleEntity schedule, CancellationToken cancellationToken = default);

        Task<WeeklyScheduleEntity?> UpdateAsync(
            Guid id,
            Guid classroomId,
            short dayId,
            TimeOnly startTime,
            TimeOnly endTime,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}