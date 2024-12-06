using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroApp.Api
{
    public static class ApiFunctions
    {
        public static List<TEnum> ConvertStringListToEnumList<TEnum>(List<string> values) where TEnum : struct, Enum
        {
            if (values == null || !values.Any())
            {
                return new List<TEnum>();
            }

            return values
            .Select(value => Enum.TryParse<TEnum>(value, true, out var result) ? result : default)
            .Where(enumValue => !EqualityComparer<TEnum>.Default.Equals(enumValue, default))
            .ToList();
        }
    }
}
