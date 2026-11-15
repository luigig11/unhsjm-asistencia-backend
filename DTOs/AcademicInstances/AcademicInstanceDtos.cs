using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.AcademicInstances
{
    public sealed class AcademicInstanceListItemDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_periodo_escolar")]
        public Guid SchoolPeriodId { get; set; }

        [JsonPropertyName("nombre_periodo_escolar")]
        public string SchoolPeriodName { get; set; } = string.Empty;

        [JsonPropertyName("id_detalle_plan_academico")]
        public Guid AcademicPlanDetailId { get; set; }

        [JsonPropertyName("plan_codigo")]
        public string AcademicPlanCode { get; set; } = string.Empty;

        [JsonPropertyName("ciclo_nombre")]
        public string AcademicCycleName { get; set; } = string.Empty;

        [JsonPropertyName("periodo_nombre")]
        public string AcademicPeriodName { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("fecha_inicio")]
        public DateOnly StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateOnly EndDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class AcademicInstanceDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_periodo_escolar")]
        public Guid SchoolPeriodId { get; set; }

        [JsonPropertyName("id_detalle_plan_academico")]
        public Guid AcademicPlanDetailId { get; set; }

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("fecha_inicio")]
        public DateOnly StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateOnly EndDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class CreateAcademicInstanceDto
    {
        [JsonPropertyName("id_periodo_escolar")]
        public Guid SchoolPeriodId { get; set; }

        [JsonPropertyName("id_detalle_plan_academico")]
        public Guid AcademicPlanDetailId { get; set; }

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("fecha_inicio")]
        public DateOnly StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateOnly EndDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateAcademicInstanceDto
    {
        [JsonPropertyName("id_periodo_escolar")]
        public Guid SchoolPeriodId { get; set; }

        [JsonPropertyName("id_detalle_plan_academico")]
        public Guid AcademicPlanDetailId { get; set; }

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("fecha_inicio")]
        public DateOnly StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateOnly EndDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

}