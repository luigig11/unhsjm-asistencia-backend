using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Classrooms
{
    public sealed class ClassroomListItemDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("id_tipo_aula")]
        public Guid ClassroomTypeId { get; init; }

        [JsonPropertyName("tipo_aula_nombre")]
        public string ClassroomTypeName { get; init; } = string.Empty;

        [JsonPropertyName("codigo")]
        public string Code { get; init; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("capacidad")]
        public short Capacity { get; init; }

        [JsonPropertyName("ubicacion")]
        public string? Location { get; init; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; init; }
    }

    public sealed class ClassroomDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("id_tipo_aula")]
        public Guid ClassroomTypeId { get; init; }

        [JsonPropertyName("codigo")]
        public string Code { get; init; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("capacidad")]
        public short Capacity { get; init; }

        [JsonPropertyName("ubicacion")]
        public string? Location { get; init; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; init; }
    }

    public sealed class CreateClassroomDto
    {
        [JsonPropertyName("id_tipo_aula")]
        public Guid ClassroomTypeId { get; set; }

        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("capacidad")]
        public short Capacity { get; set; }

        [JsonPropertyName("ubicacion")]
        public string? Location { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateClassroomDto
    {
        [JsonPropertyName("id_tipo_aula")]
        public Guid ClassroomTypeId { get; set; }

        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("capacidad")]
        public short Capacity { get; set; }

        [JsonPropertyName("ubicacion")]
        public string? Location { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

}
