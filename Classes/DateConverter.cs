using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NeuroApp.Classes
{
    public class DateConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if(!string.IsNullOrEmpty(value) && value.Length >= 10)
            {
                var datePart = value.Substring(0, 10);
                if (DateTime.TryParse(datePart, out DateTime parsedDate))
                {
                    return parsedDate.Date;
                }
            }
            
            throw new JsonException($"Invalid date format: {value}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("dd-MM-yyyy"));
        }
    }
}
