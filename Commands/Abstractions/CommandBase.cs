namespace unhsjm_asistencia_backend.Commands.Abstractions
{
    public abstract class CommandBase<TResult> : ICommand<TResult>
    {
        public abstract Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default);
        public virtual Task UndoAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

    }
}
