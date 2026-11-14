using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Permissions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Permissions
{
    public sealed class ListPermissionsCommand : CommandBase<IReadOnlyList<PermissionDto>>
    {
        private readonly IPermissionRepository _permissionRepository;
        private Guid? _roleId;
        private string? _entityName;
        private bool? _isActive;

        public ListPermissionsCommand(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public ListPermissionsCommand WithFilter(Guid? roleId, string? entityName, bool? isActive)
        {
            _roleId = roleId;
            _entityName = entityName;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<PermissionDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var permissions = await _permissionRepository.GetAllAsync(_roleId, _entityName, _isActive, cancellationToken);
            return permissions
                .Select(p => new PermissionDto
                {
                    Id = p.Id,
                    RoleId = p.RoleId,
                    EntityName = p.EntityName,
                    AllowedOperations = p.AllowedOperations,
                    CreatedAt = TimeZoneHelper.ToManagua(p.CreatedAt),
                    UpdatedAt = TimeZoneHelper.ToManagua(p.UpdatedAt),
                    IsActive = p.IsActive
                })
                .ToList();
        }
    }
}
