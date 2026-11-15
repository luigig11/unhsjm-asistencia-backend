using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.GroupEnrollments
{
    public sealed class GroupEnrollmentListItemDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_materia_por_grupo")]
        public Guid GroupSubjectId { get; set; }

        [JsonPropertyName("id_estudiante")]
        public Guid StudentId { get; set; }

        [JsonPropertyName("codigo_unico")]
        public string UniqueCode { get; set; } = string.Empty;

        [JsonPropertyName("carnet")]
        public string StudentCarnet { get; set; } = string.Empty;

        [JsonPropertyName("nombres")]
        public string FirstNames { get; set; } = string.Empty;

        [JsonPropertyName("primer_apellido")]
        public string FirstLastName { get; set; } = string.Empty;

        [JsonPropertyName("segundo_apellido")]
        public string? SecondLastName { get; set; }

        [JsonPropertyName("fecha_inscripcion")]
        public DateOnly EnrollmentDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class GroupEnrollmentDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_materia_por_grupo")]
        public Guid GroupSubjectId { get; set; }

        [JsonPropertyName("id_estudiante")]
        public Guid StudentId { get; set; }

        [JsonPropertyName("fecha_inscripcion")]
        public DateOnly EnrollmentDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class CreateGroupEnrollmentDto
    {
        [JsonPropertyName("id_materia_por_grupo")]
        public Guid GroupSubjectId { get; set; }

        [JsonPropertyName("id_estudiante")]
        public Guid StudentId { get; set; }

        [JsonPropertyName("fecha_inscripcion")]
        public DateOnly? EnrollmentDate { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateGroupEnrollmentDto
    {
        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }
}