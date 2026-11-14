using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Students
{
    public sealed class StudentListItemDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("codigo_unico")]
        public string UniqueCode { get; init; } = string.Empty;

        [JsonPropertyName("carnet")]
        public string StudentCarnet { get; init; } = string.Empty;

        [JsonPropertyName("cedula")]
        public string? NationalId { get; init; }

        [JsonPropertyName("nombres")]
        public string FirstNames { get; init; } = string.Empty;

        [JsonPropertyName("primer_apellido")]
        public string FirstLastName { get; init; } = string.Empty;

        [JsonPropertyName("segundo_apellido")]
        public string? SecondLastName { get; init; }

        [JsonPropertyName("correo")]
        public string? Email { get; init; }

        [JsonPropertyName("telefono")]
        public string? Phone { get; init; }

        [JsonPropertyName("fecha_ingreso")]
        public DateOnly AdmissionDate { get; init; }

        [JsonPropertyName("id_carrera")]
        public Guid CareerId { get; init; }

        [JsonPropertyName("carrera_nombre")]
        public string CareerName { get; init; } = string.Empty;

        [JsonPropertyName("id_plan_academico")]
        public Guid AcademicPlanId { get; init; }

        [JsonPropertyName("plan_codigo")]
        public string AcademicPlanCode { get; init; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool IsActive { get; init; }
    }
}
