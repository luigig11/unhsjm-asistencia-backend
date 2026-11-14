using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Permissions
{
    public sealed class CreatePermissionDto
    {
        [JsonPropertyName("id_rol")]
        public Guid RoleId { get; set; }

        [JsonPropertyName("entidad")]
        public string EntityName { get; set; } = string.Empty;

        [JsonPropertyName("operaciones_permitidas")]
        public IReadOnlyList<string> AllowedOperations { get; set; } = [];

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }
}
