using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Dashboard
{
    public sealed class DashboardSummaryResponseDto
    {
        [JsonPropertyName("periodo_escolar_activo")]
        public DashboardSchoolPeriodDto ActiveSchoolPeriod { get; set; } = new();

        [JsonPropertyName("total_grupos_ofertados")]
        public int TotalOfferedGroupSubjects { get; set; }

        [JsonPropertyName("total_sesiones_hoy")]
        public int TotalSessionsToday { get; set; }

        [JsonPropertyName("alertas_ausencia_docente_hoy")]
        public int TeacherAbsenceAlertsToday { get; set; }

        [JsonPropertyName("sesiones_hoy")]
        public IReadOnlyList<DashboardTodaySessionDto> SessionsToday { get; set; } = [];

        [JsonPropertyName("proximos_eventos")]
        public IReadOnlyList<DashboardUpcomingEventDto> UpcomingEvents { get; set; } = [];
    }

    public sealed class DashboardSchoolPeriodDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;
    }

    public sealed class DashboardTodaySessionDto
    {
        [JsonPropertyName("id_calendario")]
        public Guid CalendarSessionId { get; set; }

        [JsonPropertyName("materia_nombre")]
        public string SubjectName { get; set; } = string.Empty;

        [JsonPropertyName("grupo_codigo")]
        public string GroupCode { get; set; } = string.Empty;

        [JsonPropertyName("aula_codigo")]
        public string ClassroomCode { get; set; } = string.Empty;

        [JsonPropertyName("docente_nombre")]
        public string TeacherName { get; set; } = string.Empty;

        [JsonPropertyName("fecha_inicio")]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateTimeOffset EndDate { get; set; }

        [JsonPropertyName("estado_acta")]
        public string AttendanceStatus { get; set; } = string.Empty;
    }

    public sealed class DashboardUpcomingEventDto
    {
        [JsonPropertyName("id_feriado")]
        public Guid HolidayId { get; set; }

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("tipo_evento")]
        public string EventType { get; set; } = string.Empty;

        [JsonPropertyName("fecha_inicio")]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateTimeOffset EndDate { get; set; }
    }
}