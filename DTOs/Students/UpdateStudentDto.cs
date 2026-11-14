using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Students
{
    public sealed class UpdateStudentDto
    {
        [JsonPropertyName("codigo_unico")]
        public string UniqueCode { get; set; } = string.Empty;

        [JsonPropertyName("id_carrera")]
        public Guid CareerId { get; set; }

        [JsonPropertyName("id_plan_academico")]
        public Guid AcademicPlanId { get; set; }

        [JsonPropertyName("carnet")]
        public string StudentCarnet { get; set; } = string.Empty;

        [JsonPropertyName("cedula")]
        public string? NationalId { get; set; }

        [JsonPropertyName("nombres")]
        public string FirstNames { get; set; } = string.Empty;

        [JsonPropertyName("primer_apellido")]
        public string FirstLastName { get; set; } = string.Empty;

        [JsonPropertyName("segundo_apellido")]
        public string? SecondLastName { get; set; }

        [JsonPropertyName("correo")]
        public string? Email { get; set; }

        [JsonPropertyName("telefono")]
        public string? Phone { get; set; }

        [JsonPropertyName("fecha_ingreso")]
        public DateOnly? AdmissionDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }
}
