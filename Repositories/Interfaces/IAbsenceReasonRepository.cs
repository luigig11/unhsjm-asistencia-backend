using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IAbsenceReasonRepository
    {
        Task<IReadOnlyList<AbsenceReasonEntity>> GetAllAsync(string? appliesTo, bool? isActive, CancellationToken cancellationToken = default);
        Task<AbsenceReasonEntity> CreateAsync(AbsenceReasonEntity absenceReason, CancellationToken cancellationToken = default);
        Task<AbsenceReasonEntity?> UpdateAsync(Guid id, AbsenceReasonEntity absenceReason, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}