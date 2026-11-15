using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Holidays
{
    public sealed class HolidayListItemDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_periodo_escolar")]
        public Guid SchoolPeriodId { get; set; }

        [JsonPropertyName("id_tipo_evento")]
        public Guid EventTypeId { get; set; }

        [JsonPropertyName("tipo_evento_codigo")]
        public string EventTypeCode { get; set; } = string.Empty;

        [JsonPropertyName("tipo_evento_nombre")]
        public string EventTypeName { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("fecha_inicio")]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateTimeOffset EndDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class HolidayDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_periodo_escolar")]
        public Guid SchoolPeriodId { get; set; }

        [JsonPropertyName("id_tipo_evento")]
        public Guid EventTypeId { get; set; }

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("fecha_inicio")]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateTimeOffset EndDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class CreateHolidayDto
    {
        [JsonPropertyName("id_periodo_escolar")]
        public Guid SchoolPeriodId { get; set; }

        [JsonPropertyName("id_tipo_evento")]
        public Guid EventTypeId { get; set; }

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("fecha_inicio")]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateTimeOffset EndDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateHolidayDto
    {
        [JsonPropertyName("id_periodo_escolar")]
        public Guid SchoolPeriodId { get; set; }

        [JsonPropertyName("id_tipo_evento")]
        public Guid EventTypeId { get; set; }

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("fecha_inicio")]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateTimeOffset EndDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }
}