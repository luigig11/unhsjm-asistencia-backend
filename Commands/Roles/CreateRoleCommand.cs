using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;
using unhsjm_asistencia_backend.DTOs.Roles;

namespace unhsjm_asistencia_backend.Commands.Roles
{
    public sealed class CreateRoleCommand : CommandBase<RoleDto>
    {
        private readonly IDapperRlsContext _dbContext;
        private readonly IRoleRepository _roleRepository;
        private CreateRoleDto? _request;

        public CreateRoleCommand(IDapperRlsContext dbContext, IRoleRepository roleRepository)
        {
            _dbContext = dbContext;
            _roleRepository = roleRepository;
        }

        public CreateRoleCommand WithRequest(CreateRoleDto request)
        {
            _request = request;
            return this;
        }

        public override async Task<RoleDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido");
            ValidateRequest(dto.Code, dto.Name);

            await _dbContext.BeginTransactionAsync(cancellationToken: cancellationToken);

            var created = await _roleRepository.CreateAsync(
                new RoleEntity
                {
                    Code = dto.Code.Trim().ToUpperInvariant(),
                    Name = dto.Name.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken);

            await _dbContext.CommitTransactionAsync(cancellationToken);

            return new RoleDto
            {
                Id = created.Id,
                Code = created.Code,
                Name = created.Name,
                IsActive = created.IsActive
            };
        }

        public override async Task UndoAsync(CancellationToken cancellationToken = default)
        {
            if (_dbContext.HasActiveTransaction)
            {
                await _dbContext.RollbackTransactionAsync(cancellationToken);
            }
        }

        private static void ValidateRequest(string? code, string? name)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(code) || code.Trim().Length > 30)
            {
                errors["code"] = ["Codigo de rol requerido y maximo de 30 carateres"];
            }

            if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 80)
            {
                errors["nombre"] = ["nombre de rol requerido y menor de 80 caracteres."];
            }

            if (errors.Count > 0)
            {
                throw new ValidationApiException("Ocurrieron uno o mas errores de validacion", errors);
            }
        }
    }
}
