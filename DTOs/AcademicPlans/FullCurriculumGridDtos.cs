using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.AcademicPlans
{
    public sealed class FullCurriculumGridDto
    {
        [JsonPropertyName("plan")]
        public AcademicPlanListItemDto Plan { get; set; } = new();

        [JsonPropertyName("ciclos")]
        public IReadOnlyList<CurriculumCycleNodeDto> Cycles { get; set; } = [];
    }

    public sealed class CurriculumCycleNodeDto
    {
        [JsonPropertyName("id_ciclo_academico")]
        public Guid AcademicCycleId { get; set; }

        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("orden")]
        public short Order { get; set; }

        [JsonPropertyName("bloques")]
        public IReadOnlyList<CurriculumBlockNodeDto> Blocks { get; set; } = [];
    }

    public sealed class CurriculumBlockNodeDto
    {
        [JsonPropertyName("id_detalle_plan_academico")]
        public Guid AcademicPlanDetailId { get; set; }

        [JsonPropertyName("id_periodo_academico")]
        public Guid AcademicPeriodId { get; set; }

        [JsonPropertyName("periodo_codigo")]
        public string PeriodCode { get; set; } = string.Empty;

        [JsonPropertyName("periodo_nombre")]
        public string PeriodName { get; set; } = string.Empty;

        [JsonPropertyName("periodo_orden")]
        public short PeriodOrder { get; set; }

        [JsonPropertyName("duracion_semanas")]
        public short DurationWeeks { get; set; }

        [JsonPropertyName("duracion_meses")]
        public short DurationMonths { get; set; }

        [JsonPropertyName("id_detalle_predecesor")]
        public Guid? PredecessorDetailId { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }

        [JsonPropertyName("materias")]
        public IReadOnlyList<CurriculumSubjectNodeDto> Subjects { get; set; } = [];
    }

    public sealed class CurriculumSubjectNodeDto
    {
        [JsonPropertyName("id_malla_curricular")]
        public Guid CurricularGridItemId { get; set; }

        [JsonPropertyName("id_materia")]
        public Guid SubjectId { get; set; }

        [JsonPropertyName("materia_codigo")]
        public string SubjectCode { get; set; } = string.Empty;

        [JsonPropertyName("materia_nombre")]
        public string SubjectName { get; set; } = string.Empty;

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

        [JsonPropertyName("prerequisitos")]
        public IReadOnlyList<CurriculumPrerequisiteNodeDto> Prerequisites { get; set; } = [];
    }

    public sealed class CurriculumPrerequisiteNodeDto
    {
        [JsonPropertyName("id_materia_prerequisito")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_malla_curricular_materia_prerequisito")]
        public Guid PrerequisiteCurricularGridItemId { get; set; }

        [JsonPropertyName("id_materia")]
        public Guid SubjectId { get; set; }

        [JsonPropertyName("materia_codigo")]
        public string SubjectCode { get; set; } = string.Empty;

        [JsonPropertyName("materia_nombre")]
        public string SubjectName { get; set; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

}