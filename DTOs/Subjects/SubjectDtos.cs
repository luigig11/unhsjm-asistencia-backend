using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Subjects
{
    public sealed class SubjectDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("codigo")]
        public string Code { get; init; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool IsActive { get; init; }
    }

    public sealed class CreateSubjectDto
    {
        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateSubjectDto
    {
        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }
}

