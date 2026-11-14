using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Common;
using unhsjm_asistencia_backend.DTOs.Users;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Users
{
    public sealed class ListUsersCommand : CommandBase<PagedResponseDto<UserListItemDto>>
    {
        private readonly IUserRepository _userRepository;
        private UserFilterDto _filter = new();

        public ListUsersCommand(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public ListUsersCommand WithFilter(UserFilterDto filter)
        {
            _filter = filter;
            return this;
        }

        public override async Task<PagedResponseDto<UserListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var (total, items) = await _userRepository.GetPagedAsync(_filter, cancellationToken);

            return new PagedResponseDto<UserListItemDto>
            {
                Total = total,
                Items = items
                    .Select(u => new UserListItemDto
                    {
                        Id = u.Id,
                        AuthUserId = u.AuthUserId,
                        RoleId = u.RoleId,
                        RoleName = u.RoleName,
                        NationalId = u.NationalId,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Gender = u.Gender,
                        BirthDate = u.BirthDate,
                        IsActive = u.IsActive,
                        HasTeacherProfile = u.TeacherId.HasValue,
                        TeacherId = u.TeacherId
                    })
                    .ToList()
            };
        }
    }
}
