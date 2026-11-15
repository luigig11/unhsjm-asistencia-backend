using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.AcademicPlans
{
    public sealed class AcademicPlanListItemDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_centro_universitario")]
        public Guid UniversityCenterId { get; set; }

        [JsonPropertyName("centro_universitario_nombre")]
        public string UniversityCenterName { get; set; } = string.Empty;

        [JsonPropertyName("id_direccion")]
        public Guid DepartmentId { get; set; }

        [JsonPropertyName("direccion_nombre")]
        public string DepartmentName { get; set; } = string.Empty;

        [JsonPropertyName("id_carrera")]
        public Guid CareerId { get; set; }

        [JsonPropertyName("carrera_nombre")]
        public string CareerName { get; set; } = string.Empty;

        [JsonPropertyName("id_tipo_periodo_academico")]
        public Guid AcademicPeriodTypeId { get; set; }

        [JsonPropertyName("tipo_periodo_academico_nombre")]
        public string AcademicPeriodTypeName { get; set; } = string.Empty;

        [JsonPropertyName("id_modalidad")]
        public Guid ModalityId { get; set; }

        [JsonPropertyName("modalidad_nombre")]
        public string ModalityName { get; set; } = string.Empty;

        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("cantidad_ciclo_academico")]
        public short AcademicCycleCount { get; set; }

        [JsonPropertyName("cantidad_periodo_academico")]
        public short AcademicPeriodCount { get; set; }

        [JsonPropertyName("activo_desde")]
        public DateOnly ActiveFrom { get; set; }

        [JsonPropertyName("activo_hasta")]
        public DateOnly? ActiveUntil { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class AcademicPlanDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_centro_universitario")]
        public Guid UniversityCenterId { get; set; }

        [JsonPropertyName("id_carrera")]
        public Guid CareerId { get; set; }

        [JsonPropertyName("id_tipo_periodo_academico")]
        public Guid AcademicPeriodTypeId { get; set; }

        [JsonPropertyName("id_modalidad")]
        public Guid ModalityId { get; set; }

        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("cantidad_ciclo_academico")]
        public short AcademicCycleCount { get; set; }

        [JsonPropertyName("cantidad_periodo_academico")]
        public short AcademicPeriodCount { get; set; }

        [JsonPropertyName("activo_desde")]
        public DateOnly ActiveFrom { get; set; }

        [JsonPropertyName("activo_hasta")]
        public DateOnly? ActiveUntil { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class CreateAcademicPlanDto
    {
        [JsonPropertyName("id_centro_universitario")]
        public Guid UniversityCenterId { get; set; }

        [JsonPropertyName("id_carrera")]
        public Guid CareerId { get; set; }

        [JsonPropertyName("id_tipo_periodo_academico")]
        public Guid AcademicPeriodTypeId { get; set; }

        [JsonPropertyName("id_modalidad")]
        public Guid ModalityId { get; set; }

        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("cantidad_ciclo_academico")]
        public short AcademicCycleCount { get; set; }

        [JsonPropertyName("cantidad_periodo_academico")]
        public short AcademicPeriodCount { get; set; }

        [JsonPropertyName("activo_desde")]
        public DateOnly ActiveFrom { get; set; }

        [JsonPropertyName("activo_hasta")]
        public DateOnly? ActiveUntil { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateAcademicPlanDto
    {
        [JsonPropertyName("id_centro_universitario")]
        public Guid UniversityCenterId { get; set; }

        [JsonPropertyName("id_carrera")]
        public Guid CareerId { get; set; }

        [JsonPropertyName("id_tipo_periodo_academico")]
        public Guid AcademicPeriodTypeId { get; set; }

        [JsonPropertyName("id_modalidad")]
        public Guid ModalityId { get; set; }

        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("cantidad_ciclo_academico")]
        public short AcademicCycleCount { get; set; }

        [JsonPropertyName("cantidad_periodo_academico")]
        public short AcademicPeriodCount { get; set; }

        [JsonPropertyName("activo_desde")]
        public DateOnly ActiveFrom { get; set; }

        [JsonPropertyName("activo_hasta")]
        public DateOnly? ActiveUntil { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }
}