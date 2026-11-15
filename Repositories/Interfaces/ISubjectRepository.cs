using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface ISubjectRepository
    {
        Task<IReadOnlyList<SubjectEntity>> GetAllAsync(string? search, bool? isActive, CancellationToken cancellationToken = default);
        Task<SubjectEntity> CreateAsync(SubjectEntity subject, CancellationToken cancellationToken = default);
        Task<SubjectEntity?> UpdateAsync(Guid id, SubjectEntity subject, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

