using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Common
{
    public sealed class ErrorResponseDto
    {
        [JsonPropertyName("status")]
        public int Status { get; init; }

        [JsonPropertyName("error")]
        public string Error { get; init; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; init; } = string.Empty;

        [JsonPropertyName("details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, string[]>? Details { get; init; }
    }
}
