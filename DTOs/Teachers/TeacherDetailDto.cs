using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Teachers
{
    public sealed class TeacherDetailDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("id_usuario")]
        public Guid UserId { get; init; }

        [JsonPropertyName("usuario")]
        public TeacherUserSummaryDto User { get; init; } = new();

        [JsonPropertyName("id_direccion")]
        public Guid DepartmentId { get; init; }

        [JsonPropertyName("codigo_empleado")]
        public string EmployeeCode { get; init; } = string.Empty;

        [JsonPropertyName("telefono")]
        public string? Phone { get; init; }

        [JsonPropertyName("tipo_contratacion")]
        public string ContractType { get; init; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool IsActive { get; init; }
    }

    public sealed class TeacherUserSummaryDto
    {
        [JsonPropertyName("nombre")]
        public string FirstName { get; init; } = string.Empty;

        [JsonPropertyName("apellido")]
        public string LastName { get; init; } = string.Empty;

        [JsonPropertyName("cedula")]
        public string? NationalId { get; init; }

        [JsonPropertyName("genero")]
        public string Gender { get; init; } = string.Empty;
    }
}
