using Supabase;

namespace unhsjm_asistencia_backend.Data
{
    public interface ISupabaseClientFactory
    {
        Task<Client> CreateClientAsync();
    }
}
