using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rest.API.Converters
{
    /// <summary>
    /// it serializes without a "Z" and browsers read it as local time. Write it as UTC explicitly.
    /// </summary>
    public class UtcDateTimeJsonConverter : JsonConverter<DateTime>
    {
        /// <summary>Reads a DateTime from JSON.</summary>
        public override DateTime Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.GetDateTime();

        /// <summary>Writes a DateTime as an explicit UTC value (with "Z").</summary>
        public override void Write(
            Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options )
        {
            var utc = value.Kind switch
            {
                DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => value,
            };

            writer.WriteStringValue(utc);
        }
    }
}
