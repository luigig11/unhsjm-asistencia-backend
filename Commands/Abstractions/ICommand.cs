namespace unhsjm_asistencia_backend.Commands.Abstractions
{
    public interface ICommand<TResult>
    {
        Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default);
        Task UndoAsync(CancellationToken cancellationToken = default);
    }
}
