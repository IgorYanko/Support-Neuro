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
            return typeof(T).GetFields()
                            .Where(f => f.IsLiteral)
                            .Select(f =>
                            {
                                var attribute = f.GetCustomAttribute<JsonPropertyNameAttribute>();
                                return attribute?.Name ?? NameMapping.GetValueOrDefault(f.Name, f.Name);
                            })
                            .ToList();
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
