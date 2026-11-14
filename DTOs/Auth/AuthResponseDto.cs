using System.Text.Json.Serialization;
namespace unhsjm_asistencia_backend.DTOs.Auth
{
    public sealed class RegisterResponseDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; } = string.Empty;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; init; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }

        [JsonPropertyName("usuario")]
        public RegisteredUserSummaryDto User { get; init; } = new();
    }

    public sealed class RegisteredUserSummaryDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("id_user")]
        public Guid? AuthUserId { get; init; }

        [JsonPropertyName("id_rol")]
        public Guid RoleId { get; init; }

        [JsonPropertyName("rol_code")]
        public string RoleCode { get; init; } = string.Empty;

        [JsonPropertyName("cedula")]
        public string? NationalId { get; init; }

        [JsonPropertyName("nombre")]
        public string FirstName { get; init; } = string.Empty;

        [JsonPropertyName("apellido")]
        public string LastName { get; init; } = string.Empty;

        [JsonPropertyName("genero")]
        public string Gender { get; init; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool IsActive { get; init; }
    }

    public sealed class LoginResponseDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; } = string.Empty;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; init; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }

        [JsonPropertyName("usuario")]
        public LoggedInUserSummaryDto User { get; init; } = new();
    }

    public sealed class LoggedInUserSummaryDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("id_user")]
        public Guid? AuthUserId { get; init; }

        [JsonPropertyName("id_rol")]
        public Guid RoleId { get; init; }

        [JsonPropertyName("rol_code")]
        public string RoleCode { get; init; } = string.Empty;

        [JsonPropertyName("id_docente")]
        public Guid? TeacherId { get; init; }

        [JsonPropertyName("nombre")]
        public string FirstName { get; init; } = string.Empty;

        [JsonPropertyName("apellido")]
        public string LastName { get; init; } = string.Empty;
    }

    public sealed class RefreshTokenResponseDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; } = string.Empty;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; init; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }
    }
}
