using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Roles;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Roles
{
    public sealed class UpdateRoleCommand : CommandBase<RoleDto>
    {
        private readonly IDapperRlsContext _dbContext;
        private readonly IRoleRepository _roleRepository;
        private Guid _roleId;
        private UpdateRoleDto? _request;

        public UpdateRoleCommand(IDapperRlsContext dbContext, IRoleRepository roleRepository)
        {
            _dbContext = dbContext;
            _roleRepository = roleRepository;
        }

        public UpdateRoleCommand WithRequest(Guid id, UpdateRoleDto request)
        {
            _roleId = id;
            _request = request;
            return this;
        }

        public override async Task<RoleDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");

            var errors = new Dictionary<string, string[]>();
            if (string.IsNullOrWhiteSpace(dto.Code) || dto.Code.Trim().Length > 30)
            {
                errors["code"] = ["Codigo de rol requerido y de menos de 30 caracteres"];
            }

            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length > 80)
            {
                errors["nombre"] = ["Nombre de rol requerido y de menos de 30 caracteres."];
            }

            if (errors.Count > 0)
            {
                throw new ValidationApiException("Una o mas validaciones fallaron.", errors);
            }

            await _dbContext.BeginTransactionAsync(cancellationToken: cancellationToken);

            var updated = await _roleRepository.UpdateAsync(
                _roleId,
                new RoleEntity
                {
                    Id = _roleId,
                    Code = dto.Code.Trim().ToUpperInvariant(),
                    Name = dto.Name.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Role with id '{_roleId}' was not found.");

            await _dbContext.CommitTransactionAsync(cancellationToken);

            return new RoleDto
            {
                Id = updated.Id,
                Code = updated.Code,
                Name = updated.Name,
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
    }
}
