using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.SchoolPeriods
{
    public sealed class SchoolPeriodDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("nombre_periodo_escolar")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("fecha_inicio")]
        public DateOnly StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateOnly EndDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class CreateSchoolPeriodDto
    {
        [JsonPropertyName("nombre_periodo_escolar")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("fecha_inicio")]
        public DateOnly StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateOnly EndDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateSchoolPeriodDto
    {
        [JsonPropertyName("nombre_periodo_escolar")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("fecha_inicio")]
        public DateOnly StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateOnly EndDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

}