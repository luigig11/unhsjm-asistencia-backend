using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Permissions
{
    public sealed class DeletePermissionCommand : CommandBase<bool>
    {
        private readonly IDapperRlsContext _dbContext;
        private readonly IPermissionRepository _permissionRepository;
        private Guid _permissionId;

        public DeletePermissionCommand(IDapperRlsContext dbContext, IPermissionRepository permissionRepository)
        {
            _dbContext = dbContext;
            _permissionRepository = permissionRepository;
        }

        public DeletePermissionCommand WithId(Guid id)
        {
            _permissionId = id;
            return this;
        }

        public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.BeginTransactionAsync(cancellationToken: cancellationToken);

            var deleted = await _permissionRepository.DeleteAsync(_permissionId, cancellationToken);
            if (!deleted)
            {
                throw new NotFoundApiException($"Permiso con id '{_permissionId}' no fue encontrado.");
            }

            await _dbContext.CommitTransactionAsync(cancellationToken);
            return true;
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
