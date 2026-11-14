using System.Security.Claims;
using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Auth;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Auth
{
    public sealed class GetCurrentProfileCommand : CommandBase<CurrentProfileResponseDto>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;

        public GetCurrentProfileCommand(
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository,
            IPermissionRepository permissionRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
        }

        public override async Task<CurrentProfileResponseDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            var sub = principal?.FindFirstValue("sub") ?? principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out var authUserId))
            {
                throw new UnauthorizedApiException("Campo 'sub' invalido en JWT.");
            }

            var email = principal?.FindFirstValue("email") ?? principal?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            var user = await _userRepository.GetByAuthUserIdAsync(authUserId, asAnonymous: false, cancellationToken: cancellationToken)
                ?? throw new NotFoundApiException("Usuario no encontrado");

            var permissions = await _permissionRepository.GetAllAsync(
                roleId: user.RoleId,
                entityName: null,
                isActive: true,
                cancellationToken: cancellationToken);

            return new CurrentProfileResponseDto
            {
                UserId = user.Id,
                AuthUserId = user.AuthUserId,
                Email = email,
                NationalId = user.NationalId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                BirthDate = user.BirthDate,
                Role = new CurrentProfileRoleDto
                {
                    Id = user.RoleId,
                    Code = user.RoleCode,
                    Name = user.RoleName
                },
                Teacher = user.TeacherId.HasValue
                    ? new CurrentProfileTeacherDto
                    {
                        Id = user.TeacherId.Value,
                        EmployeeCode = user.EmployeeCode ?? string.Empty,
                        DepartmentId = user.DepartmentId ?? Guid.Empty,
                        ContractType = user.ContractType ?? string.Empty
                    }
                    : null,
                Permissions = permissions
                    .Select(p => new CurrentProfilePermissionDto
                    {
                        EntityName = p.EntityName,
                        AllowedOperations = p.AllowedOperations
                    })
                    .ToList()
            };
        }
    }


}
