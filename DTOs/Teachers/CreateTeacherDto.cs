using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Teachers
{
    public sealed class CreateTeacherDto
    {
        [JsonPropertyName("id_usuario")]
        public Guid UserId { get; set; }

        [JsonPropertyName("id_direccion")]
        public Guid DepartmentId { get; set; }

        [JsonPropertyName("codigo_empleado")]
        public string EmployeeCode { get; set; } = string.Empty;

        [JsonPropertyName("telefono")]
        public string? Phone { get; set; }

        [JsonPropertyName("tipo_contratacion")]
        public string ContractType { get; set; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }
}
