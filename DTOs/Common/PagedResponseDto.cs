using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Common
{
    public sealed class PagedResponseDto<T>
    {
        [JsonPropertyName("total")]
        public int Total { get; init; }

        [JsonPropertyName("items")]
        public IReadOnlyList<T> Items { get; init; } = [];
    }
}
