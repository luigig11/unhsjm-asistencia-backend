using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Users
{
    public sealed class UpdateUserDto
    {
        [JsonPropertyName("id_rol")]
        public Guid RoleId { get; set; }

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

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UserDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("id_user")]
        public Guid? AuthUserId { get; init; }

        [JsonPropertyName("id_rol")]
        public Guid RoleId { get; init; }

        [JsonPropertyName("cedula")]
        public string? NationalId { get; init; }

        [JsonPropertyName("nombre")]
        public string FirstName { get; init; } = string.Empty;

        [JsonPropertyName("apellido")]
        public string LastName { get; init; } = string.Empty;

        [JsonPropertyName("genero")]
        public string Gender { get; init; } = string.Empty;

        [JsonPropertyName("fecha_nacimiento")]
        public DateOnly? BirthDate { get; init; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; init; }
    }


}
