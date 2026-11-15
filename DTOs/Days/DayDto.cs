using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Days
{
    public sealed class DayDto
    {
        [JsonPropertyName("id")]
        public short Id { get; init; }

        [JsonPropertyName("nombre")]
        public string Name { get; init; } = string.Empty;
    }
}

