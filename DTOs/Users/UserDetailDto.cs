using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Users
{
    public sealed class UserDetailDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("id_user")]
        public Guid? AuthUserId { get; init; }

        [JsonPropertyName("id_rol")]
        public Guid RoleId { get; init; }

        [JsonPropertyName("cedula")]
        public string? NationalId { get; init; }

        [JsonPropertyName("nombre")]
        public string FirstName { get; init; } = string.Empty;

        [JsonPropertyName("apellido")]
        public string LastName { get; init; } = string.Empty;

        [JsonPropertyName("genero")]
        public string Gender { get; init; } = string.Empty;

        [JsonPropertyName("fecha_nacimiento")]
        public DateOnly? BirthDate { get; init; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; init; }

        [JsonPropertyName("docente")]
        public UserTeacherExtensionDto? Teacher { get; init; }
    }

    public sealed class UserTeacherExtensionDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

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
