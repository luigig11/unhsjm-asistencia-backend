namespace unhsjm_asistencia_backend.Commands.Abstractions
{
    public sealed class CommandInvoker : ICommandInvoker
    {
        private readonly ILogger<CommandInvoker> _logger;

        public CommandInvoker(ILogger<CommandInvoker> logger)
        {
            _logger = logger;
        }

        public async Task<TResult> InvokeAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        {
            try
            {
                return await command.ExecuteAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "El comando {CommandType} fallo en la ejecucion. Ejecutando UndoAsync.", command.GetType().Name);
                await command.UndoAsync(cancellationToken);
                throw;
            }
        }
    }
}
