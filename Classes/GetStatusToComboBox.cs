  using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NeuroApp.Classes
{
    public static class GetStatusToComboBox
    {
        private static readonly Dictionary<string, string> NameMapping = new()
        {
            { "Emexecução", "Em Execução" },
            { "ControledeQualidade", "Controle de Qualidade" },
            { "AprovadoQualidade", "Aprovado na Qualidade" },
            { "ReprovadoQualidade", "Reprovado na Qualidade" },
            { "EsperandoColeta", "Esperando Coleta" }
        };

        public static List<string> GetStatusToComboBoxList<T>() where T : Enum
        {
            var list = new List<string>();

            foreach (var field in typeof(T).GetFields().Where(f => f.IsLiteral))
            {
                var attribute = field.GetCustomAttribute<JsonPropertyNameAttribute>();
                if (attribute != null)
                {
                    list.Add(attribute.Name);
                }
                else if (NameMapping.TryGetValue(field.Name, out string? mappedValue))
                {
                    list.Add(mappedValue);
                }
                else
                {
                    list.Add(field.Name);
                }
            }

            return list;
        }

        public static T? ConvertDisplayToEnum<T>(string displayValue) where T : struct, Enum
        {
            var reverseMapping = NameMapping.ToDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

            if (reverseMapping.TryGetValue(displayValue, out string? originalEnumName))
            {
                if (Enum.TryParse(originalEnumName, out T parsedEnum))
                    return parsedEnum;
            }

            if (Enum.TryParse(displayValue, out T directEnum))
                return directEnum;

            return null;
        }
    }
}
