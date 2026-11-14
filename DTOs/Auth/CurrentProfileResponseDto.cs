using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Auth
{
    public sealed class CurrentProfileResponseDto
    {
        [JsonPropertyName("id_usuario")]
        public Guid UserId { get; init; }

        [JsonPropertyName("id_user")]
        public Guid? AuthUserId { get; init; }

        [JsonPropertyName("email")]
        public string Email { get; init; } = string.Empty;

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

        [JsonPropertyName("rol")]
        public CurrentProfileRoleDto Role { get; init; } = new();

        [JsonPropertyName("docente")]
        public CurrentProfileTeacherDto? Teacher { get; init; }

        [JsonPropertyName("permisos")]
        public IReadOnlyList<CurrentProfilePermissionDto> Permissions { get; init; } = [];
    }

    public sealed class CurrentProfileRoleDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("code")]
        public string Code { get; init; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; init; } = string.Empty;
    }

    public sealed class CurrentProfileTeacherDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("codigo_empleado")]
        public string EmployeeCode { get; init; } = string.Empty;

        [JsonPropertyName("id_direccion")]
        public Guid DepartmentId { get; init; }

        [JsonPropertyName("tipo_contratacion")]
        public string ContractType { get; init; } = string.Empty;
    }

    public sealed class CurrentProfilePermissionDto
    {
        [JsonPropertyName("entidad")]
        public string EntityName { get; init; } = string.Empty;

        [JsonPropertyName("operaciones_permitidas")]
        public IReadOnlyList<string> AllowedOperations { get; init; } = [];
    }
}
