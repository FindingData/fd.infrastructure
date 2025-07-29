using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T? value) where T : struct, Enum
        {
            if (value == null)
                return string.Empty;

            var enumValue = value.Value;
            var field = enumValue.GetType().GetField(enumValue.ToString());
            var descAttr = field?.GetCustomAttribute<DescriptionAttribute>();

            return descAttr?.Description ?? enumValue.ToString();
        }
    }
}
