using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IGroupRepository
    {
        Task<IReadOnlyList<GroupWithInstanceEntity>> GetAllAsync(
            Guid? academicInstanceId,
            bool? isActive,
            CancellationToken cancellationToken = default);

        Task<GroupEntity> CreateAsync(GroupEntity group, CancellationToken cancellationToken = default);
        Task<GroupEntity?> UpdateAsync(Guid id, GroupEntity group, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}