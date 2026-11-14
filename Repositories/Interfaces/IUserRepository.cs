using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.DTOs.Users;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<Guid> InsertAnonUserAsync(UserEntity user, CancellationToken cancellationToken = default);
        Task<UserWithDetailsEntity?> GetByAuthUserIdAsync(Guid authUserId, bool asAnonymous = false, CancellationToken cancellationToken = default);
        Task<(int Total, IReadOnlyList<UserWithDetailsEntity> Items)> GetPagedAsync(UserFilterDto filter, CancellationToken cancellationToken = default);
        Task<UserWithDetailsEntity?> GetByIdWithTeacherAsync(Guid id, CancellationToken cancellationToken = default);
        Task<UserEntity?> UpdateAsync(Guid id, UserEntity user, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
