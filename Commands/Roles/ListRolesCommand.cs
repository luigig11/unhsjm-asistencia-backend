using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Roles;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Roles
{
    public sealed class ListRolesCommand : CommandBase<IReadOnlyList<RoleDto>>
    {
        private readonly IRoleRepository _roleRepository;
        private bool? _isActive;

        public ListRolesCommand(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public ListRolesCommand WithFilter(bool? isActive)
        {
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<RoleDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var roles = await _roleRepository.GetAllAsync(_isActive, cancellationToken);
            return roles
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Code = r.Code,
                    Name = r.Name,
                    IsActive = r.IsActive
                })
                .ToList();
        }
    }

}
