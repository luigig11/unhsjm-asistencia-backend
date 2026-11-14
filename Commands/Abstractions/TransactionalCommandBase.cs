using unhsjm_asistencia_backend.Data;

namespace unhsjm_asistencia_backend.Commands.Abstractions
{
    public abstract class TransactionalCommandBase<TResult> : CommandBase<TResult>
    {
        protected IDapperRlsContext DbContext { get; }

        protected TransactionalCommandBase(IDapperRlsContext dbContext)
        {
            DbContext = dbContext;
        }

        public sealed override async Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            ValidateBeforeTransaction();

            await DbContext.BeginTransactionAsync(cancellationToken: cancellationToken);
            var result = await ExecuteInTransactionAsync(cancellationToken);
            await DbContext.CommitTransactionAsync(cancellationToken);

            return result;
        }

        public override async Task UndoAsync(CancellationToken cancellationToken = default)
        {
            if (DbContext.HasActiveTransaction)
            {
                await DbContext.RollbackTransactionAsync(cancellationToken);
            }
        }

        protected virtual void ValidateBeforeTransaction()
        {
        }

        protected abstract Task<TResult> ExecuteInTransactionAsync(CancellationToken cancellationToken);
    }
}
