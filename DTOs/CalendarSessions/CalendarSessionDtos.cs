using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.CalendarSessions
{
    public sealed class CalendarSessionListItemDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_materia_por_grupo")]
        public Guid GroupSubjectId { get; set; }

        [JsonPropertyName("id_aula")]
        public Guid ClassroomId { get; set; }

        [JsonPropertyName("aula_codigo")]
        public string ClassroomCode { get; set; } = string.Empty;

        [JsonPropertyName("fecha_inicio")]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateTimeOffset EndDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }

        [JsonPropertyName("estado_asistencia")]
        public string AttendanceStatus { get; set; } = "PENDIENTE";
    }

    public sealed class CalendarSessionDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_materia_por_grupo")]
        public Guid GroupSubjectId { get; set; }

        [JsonPropertyName("id_aula")]
        public Guid ClassroomId { get; set; }

        [JsonPropertyName("fecha_inicio")]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateTimeOffset EndDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class CreateCalendarSessionDto
    {
        [JsonPropertyName("id_materia_por_grupo")]
        public Guid GroupSubjectId { get; set; }

        [JsonPropertyName("id_aula")]
        public Guid ClassroomId { get; set; }

        [JsonPropertyName("fecha_inicio")]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateTimeOffset EndDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateCalendarSessionDto
    {
        [JsonPropertyName("id_aula")]
        public Guid ClassroomId { get; set; }

        [JsonPropertyName("fecha_inicio")]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateTimeOffset EndDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class GeneratedCalendarResponseDto
    {
        [JsonPropertyName("sesiones_creadas")]
        public int SessionsCreated { get; set; }

        [JsonPropertyName("fechas_omitidas_por_feriado")]
        public IReadOnlyList<OmittedHolidayDateDto> OmittedHolidayDates { get; set; } = [];

        [JsonPropertyName("sesiones")]
        public IReadOnlyList<GeneratedSessionItemDto> Sessions { get; set; } = [];
    }

    public sealed class OmittedHolidayDateDto
    {
        [JsonPropertyName("fecha")]
        public DateOnly Date { get; set; }

        [JsonPropertyName("motivo_feriado")]
        public string HolidayReason { get; set; } = string.Empty;
    }

    public sealed class GeneratedSessionItemDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_materia_por_grupo")]
        public Guid GroupSubjectId { get; set; }

        [JsonPropertyName("id_aula")]
        public Guid ClassroomId { get; set; }

        [JsonPropertyName("aula_codigo")]
        public string ClassroomCode { get; set; } = string.Empty;

        [JsonPropertyName("fecha_inicio")]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateTimeOffset EndDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }


}

