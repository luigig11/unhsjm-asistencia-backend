using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Permissions;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Permissions
{
    public sealed class UpdatePermissionCommand : CommandBase<PermissionDto>
    {
        private static readonly HashSet<string> ValidOperations = new(StringComparer.OrdinalIgnoreCase)
        {
            "select", "insert", "update", "delete"
        };

        private readonly IDapperRlsContext _dbContext;
        private readonly IPermissionRepository _permissionRepository;
        private Guid _permissionId;
        private UpdatePermissionDto? _request;

        public UpdatePermissionCommand(IDapperRlsContext dbContext, IPermissionRepository permissionRepository)
        {
            _dbContext = dbContext;
            _permissionRepository = permissionRepository;
        }

        public UpdatePermissionCommand WithRequest(Guid id, UpdatePermissionDto request)
        {
            _permissionId = id;
            _request = request;
            return this;
        }

        public override async Task<PermissionDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");

            if (dto.AllowedOperations is null || dto.AllowedOperations.Count == 0
                || dto.AllowedOperations.Any(op => string.IsNullOrWhiteSpace(op) || !ValidOperations.Contains(op.Trim())))
            {
                throw new ValidationApiException(
                    "Tipo de operacion no permitida.",
                    new Dictionary<string, string[]>
                    {
                        ["operaciones_permitidas"] = ["Las operaciones permitidas son 'select', 'insert', 'update', 'delete'."]
                    });
            }

            var normalizedOperations = dto.AllowedOperations
                .Select(op => op.Trim().ToLowerInvariant())
                .Distinct()
                .ToArray();

            await _dbContext.BeginTransactionAsync(cancellationToken: cancellationToken);

            var updated = await _permissionRepository.UpdateAsync(
                _permissionId,
                new PermissionEntity
                {
                    Id = _permissionId,
                    AllowedOperations = normalizedOperations,
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Permiso con id '{_permissionId}' no fue encontrado.");

            await _dbContext.CommitTransactionAsync(cancellationToken);

            return new PermissionDto
            {
                Id = updated.Id,
                RoleId = updated.RoleId,
                EntityName = updated.EntityName,
                AllowedOperations = updated.AllowedOperations,
                CreatedAt = TimeZoneHelper.ToManagua(updated.CreatedAt),
                UpdatedAt = TimeZoneHelper.ToManagua(updated.UpdatedAt),
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
