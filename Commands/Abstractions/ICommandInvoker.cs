namespace unhsjm_asistencia_backend.Commands.Abstractions
{
    public interface ICommandInvoker
    {
        Task<TResult> InvokeAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
    }
}
