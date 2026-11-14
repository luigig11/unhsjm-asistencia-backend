using unhsjm_asistencia_backend.Entities;
namespace unhsjm_asistencia_backend.Repositories.Interfaces
{
    public interface IPermissionRepository
    {
        Task<IReadOnlyList<PermissionEntity>> GetAllAsync(
            Guid? roleId = null,
            string? entityName = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);

        Task<PermissionEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PermissionEntity> CreateAsync(PermissionEntity permission, CancellationToken cancellationToken = default);
        Task<PermissionEntity?> UpdateAsync(Guid id, PermissionEntity permission, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
