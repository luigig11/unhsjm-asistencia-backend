using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Permissions
{
    public sealed class UpdatePermissionDto
    {
        [JsonPropertyName("operaciones_permitidas")]
        public IReadOnlyList<string> AllowedOperations { get; set; } = [];

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }
}
