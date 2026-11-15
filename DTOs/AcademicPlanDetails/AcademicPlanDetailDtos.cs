using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.AcademicPlanDetails
{
    public sealed class AcademicPlanDetailListItemDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_plan_academico")]
        public Guid AcademicPlanId { get; set; }

        [JsonPropertyName("id_ciclo_academico")]
        public Guid AcademicCycleId { get; set; }

        [JsonPropertyName("ciclo_academico_codigo")]
        public string AcademicCycleCode { get; set; } = string.Empty;

        [JsonPropertyName("ciclo_academico_nombre")]
        public string AcademicCycleName { get; set; } = string.Empty;

        [JsonPropertyName("ciclo_academico_orden")]
        public short AcademicCycleOrder { get; set; }

        [JsonPropertyName("id_periodo_academico")]
        public Guid AcademicPeriodId { get; set; }

        [JsonPropertyName("periodo_academico_codigo")]
        public string AcademicPeriodCode { get; set; } = string.Empty;

        [JsonPropertyName("periodo_academico_nombre")]
        public string AcademicPeriodName { get; set; } = string.Empty;

        [JsonPropertyName("periodo_academico_orden")]
        public short AcademicPeriodOrder { get; set; }

        [JsonPropertyName("duracion_semanas")]
        public short DurationWeeks { get; set; }

        [JsonPropertyName("duracion_meses")]
        public short DurationMonths { get; set; }

        [JsonPropertyName("id_detalle_predecesor")]
        public Guid? PredecessorDetailId { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class AcademicPlanDetailDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_plan_academico")]
        public Guid AcademicPlanId { get; set; }

        [JsonPropertyName("id_ciclo_academico")]
        public Guid AcademicCycleId { get; set; }

        [JsonPropertyName("id_periodo_academico")]
        public Guid AcademicPeriodId { get; set; }

        [JsonPropertyName("duracion_semanas")]
        public short DurationWeeks { get; set; }

        [JsonPropertyName("duracion_meses")]
        public short DurationMonths { get; set; }

        [JsonPropertyName("id_detalle_predecesor")]
        public Guid? PredecessorDetailId { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class CreateAcademicPlanDetailDto
    {
        [JsonPropertyName("id_plan_academico")]
        public Guid AcademicPlanId { get; set; }

        [JsonPropertyName("id_ciclo_academico")]
        public Guid AcademicCycleId { get; set; }

        [JsonPropertyName("id_periodo_academico")]
        public Guid AcademicPeriodId { get; set; }

        [JsonPropertyName("duracion_semanas")]
        public short DurationWeeks { get; set; }

        [JsonPropertyName("duracion_meses")]
        public short DurationMonths { get; set; }

        [JsonPropertyName("id_detalle_predecesor")]
        public Guid? PredecessorDetailId { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateAcademicPlanDetailDto
    {
        [JsonPropertyName("id_plan_academico")]
        public Guid AcademicPlanId { get; set; }

        [JsonPropertyName("id_ciclo_academico")]
        public Guid AcademicCycleId { get; set; }

        [JsonPropertyName("id_periodo_academico")]
        public Guid AcademicPeriodId { get; set; }

        [JsonPropertyName("duracion_semanas")]
        public short DurationWeeks { get; set; }

        [JsonPropertyName("duracion_meses")]
        public short DurationMonths { get; set; }

        [JsonPropertyName("id_detalle_predecesor")]
        public Guid? PredecessorDetailId { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }
}