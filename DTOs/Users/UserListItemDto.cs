using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Users
{
    public sealed class UserListItemDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("id_user")]
        public Guid? AuthUserId { get; init; }

        [JsonPropertyName("id_rol")]
        public Guid RoleId { get; init; }

        [JsonPropertyName("rol_nombre")]
        public string RoleName { get; init; } = string.Empty;

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

        [JsonPropertyName("tiene_perfil_docente")]
        public bool HasTeacherProfile { get; init; }

        [JsonPropertyName("id_docente")]
        public Guid? TeacherId { get; init; }
    }
}
