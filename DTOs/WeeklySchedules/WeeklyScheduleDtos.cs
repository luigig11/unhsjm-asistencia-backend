using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.WeeklySchedules
{
    public sealed class WeeklyScheduleListItemDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_materia_por_grupo")]
        public Guid GroupSubjectId { get; set; }

        [JsonPropertyName("id_aula")]
        public Guid ClassroomId { get; set; }

        [JsonPropertyName("aula_codigo")]
        public string ClassroomCode { get; set; } = string.Empty;

        [JsonPropertyName("aula_nombre")]
        public string ClassroomName { get; set; } = string.Empty;

        [JsonPropertyName("id_dia")]
        public short DayId { get; set; }

        [JsonPropertyName("dia_nombre")]
        public string DayName { get; set; } = string.Empty;

        [JsonPropertyName("hora_inicio")]
        public TimeOnly StartTime { get; set; }

        [JsonPropertyName("hora_fin")]
        public TimeOnly EndTime { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class WeeklyScheduleDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_materia_por_grupo")]
        public Guid GroupSubjectId { get; set; }

        [JsonPropertyName("id_aula")]
        public Guid ClassroomId { get; set; }

        [JsonPropertyName("id_dia")]
        public short DayId { get; set; }

        [JsonPropertyName("hora_inicio")]
        public TimeOnly StartTime { get; set; }

        [JsonPropertyName("hora_fin")]
        public TimeOnly EndTime { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class CreateWeeklyScheduleDto
    {
        [JsonPropertyName("id_materia_por_grupo")]
        public Guid GroupSubjectId { get; set; }

        [JsonPropertyName("id_aula")]
        public Guid ClassroomId { get; set; }

        [JsonPropertyName("id_dia")]
        public short DayId { get; set; }

        [JsonPropertyName("hora_inicio")]
        public TimeOnly StartTime { get; set; }

        [JsonPropertyName("hora_fin")]
        public TimeOnly EndTime { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateWeeklyScheduleDto
    {
        [JsonPropertyName("id_aula")]
        public Guid ClassroomId { get; set; }

        [JsonPropertyName("id_dia")]
        public short DayId { get; set; }

        [JsonPropertyName("hora_inicio")]
        public TimeOnly StartTime { get; set; }

        [JsonPropertyName("hora_fin")]
        public TimeOnly EndTime { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }
}