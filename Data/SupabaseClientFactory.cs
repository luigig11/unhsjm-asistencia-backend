using Microsoft.Extensions.Options;
using unhsjm_asistencia_backend.Configuration;

namespace unhsjm_asistencia_backend.Data
{
    public sealed class SupabaseClientFactory : ISupabaseClientFactory
    {
        private readonly SupabaseOptions _options;

        public SupabaseClientFactory(IOptions<SupabaseOptions> options)
        {
            _options = options.Value;
        }

        public async Task<Supabase.Client> CreateClientAsync()
        {
            var clientOptions = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = false,
                AutoRefreshToken = false
            };

            var client = new Supabase.Client(_options.Url, _options.AnonKey, clientOptions);
            await client.InitializeAsync();
            return client;
        }
    }
}
