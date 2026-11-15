using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.UniversityCenters
{
    public sealed class UniversityCenterDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("codigo")]
        public string Code { get; init; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("ubicacion")]
        public string? Location { get; init; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; init; }
    }

    public sealed class CreateUniversityCenterDto
    {
        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("ubicacion")]
        public string? Location { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateUniversityCenterDto
    {
        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("ubicacion")]
        public string? Location { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }
}

