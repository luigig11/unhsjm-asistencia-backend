using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Users;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Users
{
    public sealed class UpdateUserCommand : CommandBase<UserDto>
    {
        private readonly IDapperRlsContext _dbContext;
        private readonly IUserRepository _userRepository;
        private Guid _userId;
        private UpdateUserDto? _request;

        public UpdateUserCommand(IDapperRlsContext dbContext, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
        }

        public UpdateUserCommand WithRequest(Guid id, UpdateUserDto request)
        {
            _userId = id;
            _request = request;
            return this;
        }

        public override async Task<UserDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ValidateRequest(dto);

            await _dbContext.BeginTransactionAsync(cancellationToken: cancellationToken);

            var entityToUpdate = new UserEntity
            {
                Id = _userId,
                RoleId = dto.RoleId,
                NationalId = string.IsNullOrWhiteSpace(dto.NationalId) ? null : dto.NationalId.Trim(),
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Gender = dto.Gender.Trim().ToUpperInvariant(),
                BirthDate = dto.BirthDate,
                IsActive = dto.IsActive
            };

            var updated = await _userRepository.UpdateAsync(_userId, entityToUpdate, cancellationToken)
                ?? throw new NotFoundApiException($"Usuairo con id '{_userId}' no fue encontrado.");

            await _dbContext.CommitTransactionAsync(cancellationToken);

            return new UserDto
            {
                Id = updated.Id,
                AuthUserId = updated.AuthUserId,
                RoleId = updated.RoleId,
                NationalId = updated.NationalId,
                FirstName = updated.FirstName,
                LastName = updated.LastName,
                Gender = updated.Gender,
                BirthDate = updated.BirthDate,
                IsActive = updated.IsActive
            };
        }

        public override async Task UndoAsync(CancellationToken cancellationToken = default)
        {
            if (_dbContext.HasActiveTransaction)
            {
                await _dbContext.RollbackTransactionAsync(cancellationToken);
            }
        }

        private static void ValidateRequest(UpdateUserDto dto)
        {
            var errors = new Dictionary<string, string[]>();

            if (dto.RoleId == Guid.Empty)
            {
                errors["id_rol"] = ["rol id es requerido."];
            }

            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                errors["nombre"] = ["Nombre es requerido."];
            }

            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                errors["apellido"] = ["Apellido es requerido."];
            }

            var gender = dto.Gender?.Trim().ToUpperInvariant();
            if (gender is not ("F" or "M"))
            {
                errors["genero"] = ["Genero debe ser 'F' o 'M'."];
            }

            if (dto.BirthDate.HasValue && dto.BirthDate.Value >= TimeZoneHelper.TodayInManagua())
            {
                errors["fecha_nacimiento"] = ["Feca de nacimiento debe ser menor a fecha actual"];
            }

            if (errors.Count > 0)
            {
                throw new ValidationApiException("Uno o mas errore de validacio ocurrieron.", errors);
            }
        }
    }
}
