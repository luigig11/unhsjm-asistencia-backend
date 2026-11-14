using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Auth
{
    public sealed class RefreshTokenRequestDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
