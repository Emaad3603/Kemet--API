using System.Text.Json.Serialization;
using System.Text.Json;

namespace Kemet.APIs.Helpers
{
    public class CustomTimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Read the time string
            string timeString = reader.GetString();

            // Try parsing the 24-hour format: "hh:mm:ss"
            if (TimeSpan.TryParse(timeString, out TimeSpan timeSpan))
            {
                return timeSpan;
            }

            // Try parsing the 12-hour format with AM/PM (e.g., "07:00 AM")
            if (DateTime.TryParse(timeString, out DateTime dateTime))
            {
                return dateTime.TimeOfDay;
            }

            // If parsing fails, throw an exception
            throw new JsonException($"Unable to convert '{timeString}' to TimeSpan.");
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            // Convert TimeSpan to a string in hh:mm:ss format
            writer.WriteStringValue(value.ToString(@"hh\:mm\:ss"));
        }
    }
}
