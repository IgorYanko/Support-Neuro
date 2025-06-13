using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NeuroApp.Classes
{
    public static class SalesCacheManager
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SCN",
            "sales_cache.json");

        public static async Task SaveCacheAsync(SalesCacheData data)
        {
            var directory = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var json = JsonSerializer.Serialize(data, options);
            await File.WriteAllTextAsync(FilePath, json);
        }

        public static async Task<SalesCacheData> LoadCacheAsync()
        {
            if (!File.Exists(FilePath))
                return new SalesCacheData();

            var json = await File.ReadAllTextAsync(FilePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return JsonSerializer.Deserialize<SalesCacheData>(json, options) ?? new SalesCacheData();
        }
    }

    public class SalesCacheData
    {
        public List<Sales> Sales { get; set; } = new();
        public HashSet<string> DeletedOsCodes { get; set; } = new();
    }
}
