using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Users
{
    public sealed class GetUserByIdCommand : CommandBase<UserDetailDto>
    {
        private readonly IUserRepository _userRepository;
        private Guid _userId;

        public GetUserByIdCommand(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public GetUserByIdCommand WithId(Guid id)
        {
            _userId = id;
            return this;
        }

        public override async Task<UserDetailDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithTeacherAsync(_userId, cancellationToken)
                ?? throw new NotFoundApiException($"User with id '{_userId}' was not found.");

            return new UserDetailDto
            {
                Id = user.Id,
                AuthUserId = user.AuthUserId,
                RoleId = user.RoleId,
                NationalId = user.NationalId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                BirthDate = user.BirthDate,
                IsActive = user.IsActive,
                Teacher = user.TeacherId.HasValue
                    ? new UserTeacherExtensionDto
                    {
                        Id = user.TeacherId.Value,
                        DepartmentId = user.DepartmentId ?? Guid.Empty,
                        EmployeeCode = user.EmployeeCode ?? string.Empty,
                        Phone = user.Phone,
                        ContractType = user.ContractType ?? string.Empty,
                        IsActive = user.TeacherIsActive ?? false
                    }
                    : null
            };
        }
    }
}
