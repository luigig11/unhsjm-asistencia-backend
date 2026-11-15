using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.TeacherPortal
{
    public sealed class TeacherTodaySessionDto
    {
        [JsonPropertyName("id_calendario")]
        public Guid CalendarSessionId { get; set; }

        [JsonPropertyName("id_materia_por_grupo")]
        public Guid GroupSubjectId { get; set; }

        [JsonPropertyName("materia_codigo")]
        public string SubjectCode { get; set; } = string.Empty;

        [JsonPropertyName("materia_nombre")]
        public string SubjectName { get; set; } = string.Empty;

        [JsonPropertyName("grupo_codigo")]
        public string GroupCode { get; set; } = string.Empty;

        [JsonPropertyName("aula_codigo")]
        public string ClassroomCode { get; set; } = string.Empty;

        [JsonPropertyName("aula_ubicacion")]
        public string? ClassroomLocation { get; set; }

        [JsonPropertyName("fecha_inicio")]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateTimeOffset EndDate { get; set; }

        [JsonPropertyName("id_asistencia")]
        public Guid? AttendanceId { get; set; }

        [JsonPropertyName("acta_registrada")]
        public bool IsRegistered { get; set; }

        [JsonPropertyName("es_sustituto")]
        public bool IsSubstitute { get; set; }
    }

    public sealed class TeacherCourseDto
    {
        [JsonPropertyName("id_materia_por_grupo")]
        public Guid GroupSubjectId { get; set; }

        [JsonPropertyName("instancia_nombre")]
        public string AcademicInstanceName { get; set; } = string.Empty;

        [JsonPropertyName("grupo_codigo")]
        public string GroupCode { get; set; } = string.Empty;

        [JsonPropertyName("materia_codigo")]
        public string SubjectCode { get; set; } = string.Empty;

        [JsonPropertyName("materia_nombre")]
        public string SubjectName { get; set; } = string.Empty;

        [JsonPropertyName("total_inscritos")]
        public int TotalEnrolled { get; set; }

        [JsonPropertyName("cupo_limite")]
        public short QuotaLimit { get; set; }
    }
}