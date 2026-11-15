using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.CurricularGridItems
{
    public sealed class CurricularGridItemDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_detalle_plan_academico")]
        public Guid AcademicPlanDetailId { get; set; }

        [JsonPropertyName("id_materia")]
        public Guid SubjectId { get; set; }

        [JsonPropertyName("encuentros_totales")]
        public short TotalMeetings { get; set; }

        [JsonPropertyName("encuentros_por_semana")]
        public short MeetingsPerWeek { get; set; }

        [JsonPropertyName("cantidad_horas_materia")]
        public decimal SubjectHours { get; set; }

        [JsonPropertyName("creditos")]
        public short Credits { get; set; }

        [JsonPropertyName("obligatoria")]
        public bool IsMandatory { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }

        [JsonPropertyName("ids_malla_prerequisitos")]
        public IReadOnlyList<Guid> PrerequisiteCurricularGridItemIds { get; set; } = [];
    }

    public sealed class CreateCurricularGridItemDto
    {
        [JsonPropertyName("id_detalle_plan_academico")]
        public Guid AcademicPlanDetailId { get; set; }

        [JsonPropertyName("id_materia")]
        public Guid SubjectId { get; set; }

        [JsonPropertyName("encuentros_totales")]
        public short TotalMeetings { get; set; }

        [JsonPropertyName("encuentros_por_semana")]
        public short MeetingsPerWeek { get; set; }

        [JsonPropertyName("cantidad_horas_materia")]
        public decimal SubjectHours { get; set; }

        [JsonPropertyName("creditos")]
        public short Credits { get; set; }

        [JsonPropertyName("obligatoria")]
        public bool IsMandatory { get; set; } = true;

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;

        [JsonPropertyName("ids_malla_prerequisitos")]
        public IReadOnlyList<Guid>? PrerequisiteCurricularGridItemIds { get; set; }
    }

    public sealed class UpdateCurricularGridItemDto
    {
        [JsonPropertyName("id_detalle_plan_academico")]
        public Guid AcademicPlanDetailId { get; set; }

        [JsonPropertyName("id_materia")]
        public Guid SubjectId { get; set; }

        [JsonPropertyName("encuentros_totales")]
        public short TotalMeetings { get; set; }

        [JsonPropertyName("encuentros_por_semana")]
        public short MeetingsPerWeek { get; set; }

        [JsonPropertyName("cantidad_horas_materia")]
        public decimal SubjectHours { get; set; }

        [JsonPropertyName("creditos")]
        public short Credits { get; set; }

        [JsonPropertyName("obligatoria")]
        public bool IsMandatory { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }

        [JsonPropertyName("ids_malla_prerequisitos")]
        public IReadOnlyList<Guid>? PrerequisiteCurricularGridItemIds { get; set; }
    }
}