using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Permissions
{
    public sealed class PermissionDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("id_rol")]
        public Guid RoleId { get; init; }

        [JsonPropertyName("entidad")]
        public string EntityName { get; init; } = string.Empty;

        [JsonPropertyName("operaciones_permitidas")]
        public IReadOnlyList<string> AllowedOperations { get; init; } = [];

        [JsonPropertyName("fecha_creacion")]
        public DateTimeOffset CreatedAt { get; init; }

        [JsonPropertyName("fecha_actualizacion")]
        public DateTimeOffset UpdatedAt { get; init; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; init; }
    }
}
