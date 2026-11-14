using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Permissions;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Permissions
{
    public sealed class CreatePermissionCommand : CommandBase<PermissionDto>
    {
        private static readonly HashSet<string> ValidOperations = new(StringComparer.OrdinalIgnoreCase)
        {
            "select", "insert", "update", "delete"
        };

        private readonly IDapperRlsContext _dbContext;
        private readonly IPermissionRepository _permissionRepository;
        private CreatePermissionDto? _request;

        public CreatePermissionCommand(IDapperRlsContext dbContext, IPermissionRepository permissionRepository)
        {
            _dbContext = dbContext;
            _permissionRepository = permissionRepository;
        }

        public CreatePermissionCommand WithRequest(CreatePermissionDto request)
        {
            _request = request;
            return this;
        }

        public override async Task<PermissionDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            var normalizedOperations = ValidateAndNormalize(dto.RoleId, dto.EntityName, dto.AllowedOperations);

            await _dbContext.BeginTransactionAsync(cancellationToken: cancellationToken);

            var created = await _permissionRepository.CreateAsync(
                new PermissionEntity
                {
                    RoleId = dto.RoleId,
                    EntityName = dto.EntityName.Trim().ToLowerInvariant(),
                    AllowedOperations = normalizedOperations,
                    IsActive = dto.IsActive
                },
                cancellationToken);

            await _dbContext.CommitTransactionAsync(cancellationToken);

            return new PermissionDto
            {
                Id = created.Id,
                RoleId = created.RoleId,
                EntityName = created.EntityName,
                AllowedOperations = created.AllowedOperations,
                CreatedAt = TimeZoneHelper.ToManagua(created.CreatedAt),
                UpdatedAt = TimeZoneHelper.ToManagua(created.UpdatedAt),
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

        private static string[] ValidateAndNormalize(Guid roleId, string? entityName, IReadOnlyList<string>? allowedOperations)
        {
            var errors = new Dictionary<string, string[]>();

            if (roleId == Guid.Empty)
            {
                errors["id_rol"] = ["Id de rol valido es requerido."];
            }

            if (string.IsNullOrWhiteSpace(entityName) || entityName.Trim().Length > 60)
            {
                errors["entidad"] = ["Nombre de la entidad menor a 60 caracteres es requerido."];
            }

            if (allowedOperations is null || allowedOperations.Count == 0)
            {
                errors["operaciones_permitidas"] = ["Al menos una operacion permitida ('select', 'insert', 'update', 'delete') es requerida."];
            }
            else if (allowedOperations.Any(op => string.IsNullOrWhiteSpace(op) || !ValidOperations.Contains(op.Trim())))
            {
                errors["operaciones_permitidas"] = ["Las operaciones permitidas deben ser 'select', 'insert', 'update', or 'delete'."];
            }

            if (errors.Count > 0)
            {
                throw new ValidationApiException("Ocurrieron 1 o mas errores de validacion.", errors);
            }

            return allowedOperations!
                .Select(op => op.Trim().ToLowerInvariant())
                .Distinct()
                .ToArray();
        }
    }
}
