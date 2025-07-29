using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace fd.infrastructure.core.Extensions
{

    public static class ConvertJsonExtension
    {
        public static T DeserializeObject<T>(this string entityString)
        {
            if (string.IsNullOrEmpty(entityString))
            {
                return default(T);
            }
            if (entityString == "{}")
            {
                entityString = "[]";
            }    
            return JsonSerializer.Deserialize<T>(entityString);
        }

        public static string? Serialize(this object obj)
        {
            if (obj == null) return null;                  
            return JsonSerializer.Serialize(obj);
        }
    }
}
