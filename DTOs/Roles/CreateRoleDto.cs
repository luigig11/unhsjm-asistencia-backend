using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Roles
{
    public sealed class CreateRoleDto
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }
}
