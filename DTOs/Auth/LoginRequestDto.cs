using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Auth
{
    public sealed class LoginRequestDto
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }
}
