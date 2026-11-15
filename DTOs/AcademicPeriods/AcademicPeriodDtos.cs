using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.AcademicPeriods
{
    public sealed class AcademicPeriodListItemDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("id_tipo_periodo_academico")]
        public Guid AcademicPeriodTypeId { get; init; }

        [JsonPropertyName("tipo_periodo_nombre")]
        public string AcademicPeriodTypeName { get; init; } = string.Empty;

        [JsonPropertyName("codigo")]
        public string Code { get; init; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("orden")]
        public short Order { get; init; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; init; }
    }

    public sealed class AcademicPeriodDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("id_tipo_periodo_academico")]
        public Guid AcademicPeriodTypeId { get; init; }

        [JsonPropertyName("codigo")]
        public string Code { get; init; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("orden")]
        public short Order { get; init; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; init; }
    }

    public sealed class CreateAcademicPeriodDto
    {
        [JsonPropertyName("id_tipo_periodo_academico")]
        public Guid AcademicPeriodTypeId { get; set; }

        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("orden")]
        public short Order { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateAcademicPeriodDto
    {
        [JsonPropertyName("id_tipo_periodo_academico")]
        public Guid AcademicPeriodTypeId { get; set; }

        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("orden")]
        public short Order { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }
}