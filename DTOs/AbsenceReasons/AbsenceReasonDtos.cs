using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.AbsenceReasons
{
    public sealed class AbsenceReasonDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("codigo")]
        public string Code { get; init; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("descripcion")]
        public string? Description { get; init; }

        [JsonPropertyName("justifica_inasistencia")]
        public bool JustifiesAbsence { get; init; }

        [JsonPropertyName("aplica_a")]
        public string AppliesTo { get; init; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool IsActive { get; init; }
    }

    public sealed class CreateAbsenceReasonDto
    {
        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("descripcion")]
        public string? Description { get; set; }

        [JsonPropertyName("justifica_inasistencia")]
        public bool JustifiesAbsence { get; set; } = true;

        [JsonPropertyName("aplica_a")]
        public string AppliesTo { get; set; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateAbsenceReasonDto
    {
        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("descripcion")]
        public string? Description { get; set; }

        [JsonPropertyName("justifica_inasistencia")]
        public bool JustifiesAbsence { get; set; } = true;

        [JsonPropertyName("aplica_a")]
        public string AppliesTo { get; set; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }
}

