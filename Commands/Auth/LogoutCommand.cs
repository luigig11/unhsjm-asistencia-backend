using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;

namespace unhsjm_asistencia_backend.Commands.Auth
{
    public sealed class LogoutCommand : CommandBase<bool>
    {
        private readonly ISupabaseClientFactory _supabaseClientFactory;

        public LogoutCommand(ISupabaseClientFactory supabaseClientFactory)
        {
            _supabaseClientFactory = supabaseClientFactory;
        }

        public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var supabaseClient = await _supabaseClientFactory.CreateClientAsync();
            await supabaseClient.Auth.SignOut();
            return true;
        }
    }
}
