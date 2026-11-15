using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Groups
{
    public sealed class GroupListItemDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_instancia_academica")]
        public Guid AcademicInstanceId { get; set; }

        [JsonPropertyName("instancia_nombre")]
        public string InstanceName { get; set; } = string.Empty;

        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("cupo")]
        public short Capacity { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class GroupDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_instancia_academica")]
        public Guid AcademicInstanceId { get; set; }

        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("cupo")]
        public short Capacity { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class CreateGroupDto
    {
        [JsonPropertyName("id_instancia_academica")]
        public Guid AcademicInstanceId { get; set; }

        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("cupo")]
        public short Capacity { get; set; } = 35;

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateGroupDto
    {
        [JsonPropertyName("id_instancia_academica")]
        public Guid AcademicInstanceId { get; set; }

        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("cupo")]
        public short Capacity { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }
}