using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Teachers
{
    public sealed class TeacherDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("id_usuario")]
        public Guid UserId { get; init; }

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
}
