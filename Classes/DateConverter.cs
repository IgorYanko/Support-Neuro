using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NeuroApp.Classes
{
    public class DateConverter : JsonConverter<DateTime>
    {
        private const string DateFormat = "yyyy-MM-dd";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return default;

            var value = reader.GetString();
            if (!string.IsNullOrEmpty(value))
            {
                if (DateTime.TryParseExact(value.Substring(0, 10), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    return parsedDate;
                }
            }

            throw new JsonException($"Invalid date format: {value}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
        }
    }
}
