using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<IReadOnlyList<RoleEntity>> GetAllAsync(bool? isActive = null, CancellationToken cancellationToken = default);
        Task<RoleEntity?> GetByIdAsync(Guid id, bool asAnonymous = false, CancellationToken cancellationToken = default);
        Task<RoleEntity> CreateAsync(RoleEntity role, CancellationToken cancellationToken = default);
        Task<RoleEntity?> UpdateAsync(Guid id, RoleEntity role, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
