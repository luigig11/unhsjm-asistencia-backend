using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.AcademicPeriodTypes
{
    public sealed class AcademicPeriodTypeDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("codigo")]
        public string Code { get; init; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("duracion_meses_minimo")]
        public short MinMonthsDuration { get; init; }

        [JsonPropertyName("duracion_meses_maximo")]
        public short MaxMonthsDuration { get; init; }

        [JsonPropertyName("duracion_horas_minimo")]
        public short MinHoursDuration { get; init; }

        [JsonPropertyName("duracion_horas_maximo")]
        public short MaxHoursDuration { get; init; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; init; }
    }

    public sealed class CreateAcademicPeriodTypeDto
    {
        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("duracion_meses_minimo")]
        public short MinMonthsDuration { get; set; }

        [JsonPropertyName("duracion_meses_maximo")]
        public short MaxMonthsDuration { get; set; }

        [JsonPropertyName("duracion_horas_minimo")]
        public short MinHoursDuration { get; set; }

        [JsonPropertyName("duracion_horas_maximo")]
        public short MaxHoursDuration { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateAcademicPeriodTypeDto
    {
        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("duracion_meses_minimo")]
        public short MinMonthsDuration { get; set; }

        [JsonPropertyName("duracion_meses_maximo")]
        public short MaxMonthsDuration { get; set; }

        [JsonPropertyName("duracion_horas_minimo")]
        public short MinHoursDuration { get; set; }

        [JsonPropertyName("duracion_horas_maximo")]
        public short MaxHoursDuration { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }
}

