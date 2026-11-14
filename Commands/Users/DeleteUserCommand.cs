using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Users
{
    public sealed class DeleteUserCommand : CommandBase<bool>
    {
        private readonly IDapperRlsContext _dbContext;
        private readonly IUserRepository _userRepository;
        private Guid _userId;

        public DeleteUserCommand(IDapperRlsContext dbContext, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
        }

        public DeleteUserCommand WithId(Guid id)
        {
            _userId = id;
            return this;
        }

        public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.BeginTransactionAsync(cancellationToken: cancellationToken);

            var deleted = await _userRepository.DeleteAsync(_userId, cancellationToken);
            if (!deleted)
            {
                throw new NotFoundApiException($"Usuario con id '{_userId}' no fue encontrado.");
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
