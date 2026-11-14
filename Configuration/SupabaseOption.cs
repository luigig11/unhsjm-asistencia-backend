namespace unhsjm_asistencia_backend.Configuration
{
    public sealed class SupabaseOptions
    {
        public const string SectionName = "Supabase";

        public string Url { get; set; } = string.Empty;
        public string AnonKey { get; set; } = string.Empty;
        public string ValidIssuer { get; set; } = string.Empty;
        public string ValidAudience { get; set; } = "authenticated";
    }
}
