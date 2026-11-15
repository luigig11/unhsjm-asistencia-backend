using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IGroupSubjectRepository
    {
        Task<IReadOnlyList<GroupSubjectWithDetailsEntity>> GetAllAsync(
            Guid? groupId,
            Guid? teacherId,
            bool? isActive,
            CancellationToken cancellationToken = default);

        Task<GroupSubjectGenerationContextEntity?> GetGenerationContextAsync(
            Guid groupSubjectId,
            CancellationToken cancellationToken = default);

        Task<GroupSubjectEntity> CreateAsync(GroupSubjectEntity groupSubject, CancellationToken cancellationToken = default);

        Task<GroupSubjectEntity?> UpdateAsync(
            Guid id,
            Guid teacherId,
            short quotaLimit,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }

}