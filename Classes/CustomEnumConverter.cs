using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NeuroApp.Classes
{
    public class CustomEnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        private readonly Dictionary<string, T> _nameToValue = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<T, string> _valueToName = new Dictionary<T, string>();

        public CustomEnumConverter()
        {
            var type = typeof(T);
            foreach (var field in type.GetFields())
            {
                if (!field.IsStatic)
                    continue;

                var enumValue = (T)field.GetValue(null);
                var attribute = field.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false)
                                     .FirstOrDefault() as JsonPropertyNameAttribute;

                var name = attribute?.Name ?? field.Name;
                _nameToValue[name] = enumValue;
                _valueToName[enumValue] = name;
            }
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (value != null && _nameToValue.TryGetValue(value, out var enumValue))
                return enumValue;

            throw new JsonException($"Unable to map '{value}' to enum {typeof(T).Name}.");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (_valueToName.TryGetValue(value, out var name))
            {
                writer.WriteStringValue(name);
            }
            else
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}
