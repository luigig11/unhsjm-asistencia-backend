using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace unhsjm_asistencia_backend.Data
{
    public static class TimeZoneHelper
    {
        public static TimeZoneInfo ManaguaTimeZone { get; } = ResolveManaguaTimeZone();

        public static DateOnly TodayInManagua()
        {
            var managuaNow = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, ManaguaTimeZone);
            return DateOnly.FromDateTime(managuaNow.DateTime);
        }

        public static DateTimeOffset ToUtc(DateTimeOffset value) => value.ToUniversalTime();

        public static DateTimeOffset ToManagua(DateTimeOffset value) =>
            TimeZoneInfo.ConvertTime(value.ToUniversalTime(), ManaguaTimeZone);

        public static DateTimeOffset CombineManaguaToUtc(DateOnly date, TimeOnly time)
        {
            var localDateTime = DateTime.SpecifyKind(date.ToDateTime(time), DateTimeKind.Unspecified);
            var offset = ManaguaTimeZone.GetUtcOffset(localDateTime);
            return new DateTimeOffset(localDateTime, offset).ToUniversalTime();
        }

        public static (DateTimeOffset StartUtc, DateTimeOffset EndUtc) GetManaguaDayRangeUtc(DateOnly date)
        {
            var startUtc = CombineManaguaToUtc(date, TimeOnly.MinValue);
            var endUtc = CombineManaguaToUtc(date.AddDays(1), TimeOnly.MinValue);
            return (startUtc, endUtc);
        }

        private static TimeZoneInfo ResolveManaguaTimeZone()
        {
            if (TimeZoneInfo.TryFindSystemTimeZoneById("America/Managua", out var ianaZone))
            {
                return ianaZone;
            }

            if (TimeZoneInfo.TryFindSystemTimeZoneById("Central America Standard Time", out var windowsZone))
            {
                return windowsZone;
            }

            return TimeZoneInfo.CreateCustomTimeZone(
                "America/Managua",
                TimeSpan.FromHours(-6),
                "(UTC-06:00) Central America",
                "Central America Standard Time");
        }
    }

    public sealed partial class ManaguaDateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
    {
        [GeneratedRegex(@"(Z|[+-]\d{2}:?\d{2})$", RegexOptions.IgnoreCase)]
        private static partial Regex ExplicitOffsetRegex();

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var raw = reader.GetString();
            if (string.IsNullOrWhiteSpace(raw))
            {
                throw new JsonException("DateTimeOffset value cannot be empty.");
            }

            var trimmed = raw.Trim();
            if (ExplicitOffsetRegex().IsMatch(trimmed))
            {
                return DateTimeOffset.Parse(trimmed, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }

            var localDateTime = DateTime.Parse(trimmed, CultureInfo.InvariantCulture, DateTimeStyles.None);
            var unspecified = DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified);
            var offset = TimeZoneHelper.ManaguaTimeZone.GetUtcOffset(unspecified);
            return new DateTimeOffset(unspecified, offset);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
