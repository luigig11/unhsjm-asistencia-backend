using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Auth
{
    public sealed class RegisterRequestDto
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("id_rol")]
        public Guid RoleId { get; set; }

        [JsonPropertyName("codigo_unico")]
        public string? UniqueCode { get; set; }

        [JsonPropertyName("cedula")]
        public string? NationalId { get; set; }

        [JsonPropertyName("nombre")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("apellido")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("genero")]
        public string Gender { get; set; } = string.Empty;

        [JsonPropertyName("fecha_nacimiento")]
        public DateOnly? BirthDate { get; set; }
    }

}
