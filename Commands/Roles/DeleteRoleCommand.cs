using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Roles
{
    public sealed class DeleteRoleCommand : CommandBase<bool>
    {
        private readonly IDapperRlsContext _dbContext;
        private readonly IRoleRepository _roleRepository;
        private Guid _roleId;

        public DeleteRoleCommand(IDapperRlsContext dbContext, IRoleRepository roleRepository)
        {
            _dbContext = dbContext;
            _roleRepository = roleRepository;
        }

        public DeleteRoleCommand WithId(Guid id)
        {
            _roleId = id;
            return this;
        }

        public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.BeginTransactionAsync(cancellationToken: cancellationToken);

            var deleted = await _roleRepository.DeleteAsync(_roleId, cancellationToken);
            if (!deleted)
            {
                throw new NotFoundApiException($"Rol con id '{_roleId}' no fue encontrado.");
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
