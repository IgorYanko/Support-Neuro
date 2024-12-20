using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroApp.Api
{
    public static class ApiFunctions
    {
        public static bool AreAnyPropertiesNullOrEmpty(object obj, string defaultValue = "---")
        {
            if (obj == null) return true;

            var properties = obj.GetType().GetProperties()
                .Where(p => p.CanRead && p.CanWrite);

            foreach (var property in properties)
            {
                var value = property.GetValue(obj);

                if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    property.SetValue(obj, defaultValue);
                }
            }

            return true;
        }
    }
}
