using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.ClassroomTypes
{
    public sealed class ClassroomTypeDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("codigo")]
        public string Code { get; init; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("descripcion")]
        public string? Description { get; init; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; init; }
    }

    public sealed class CreateClassroomTypeDto
    {
        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("descripcion")]
        public string? Description { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateClassroomTypeDto
    {
        [JsonPropertyName("codigo")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("descripcion")]
        public string? Description { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }
}

