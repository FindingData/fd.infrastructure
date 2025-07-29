using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace fd.infrastructure.core.Extensions
{
    public static class ObjectExtension
    {

        public static T DicToEntity<T>(this Dictionary<string, object> dic)
        {
            return new List<Dictionary<string, object>>() { dic }.DicToList<T>().ToList()[0];
        }

        public static List<T> DicToList<T>(this List<Dictionary<string, object>> dicList)
        {
            return dicList.DicToIEnumerable<T>().ToList();
        }
        public static object DicToList(this List<Dictionary<string, object>> dicList, Type type)
        {
            return typeof(ObjectExtension).GetMethod("DicToList")
               .MakeGenericMethod(new Type[] { type })
               .Invoke(typeof(ObjectExtension), new object[] { dicList });
        }

        public static IEnumerable<T> DicToIEnumerable<T>(this List<Dictionary<string, object>> dicList)
        {
            foreach (Dictionary<string, object> dic in dicList)
            {
                T model = Activator.CreateInstance<T>();
                foreach (PropertyInfo property in model.GetType()
                    .GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!dic.TryGetValue(property.Name, out object value)) continue;
                    property.SetValue(model, value?.ToString().ChangeType(property.PropertyType), null);
                }
                yield return model;
            }
        }

        /// <summary>
        /// Converts a JsonElement object to an appropriate .NET object type.
        /// </summary>
        /// <param name="obj">The object to convert, typically a JsonElement.</param>
        /// <returns>
        /// The converted object as a .NET type. If the conversion fails, returns the exception message.
        /// Possible return types are string, float, bool, or null.
        /// </returns>
        /// <remarks>
        /// This method attempts to determine the type of the JSON element and convert it to a corresponding .NET type.
        /// It handles various JSON value kinds such as Number, String, True, False, Null, Undefined, Object, and Array.
        /// If the conversion fails, it catches the exception and returns the exception message.
        /// </remarks>
        public static object? GetObjectValue(this object? obj)
        {
            try
            {
                switch (obj)
                {
                    case null:
                        return "NULL";
                    case JsonElement jsonElement:
                        {
                            var typeOfObject = jsonElement.ValueKind;
                            var rawText = jsonElement.GetRawText(); // Retrieves the raw JSON text for the element.

                            return typeOfObject switch
                            {
                                JsonValueKind.Number => float.Parse(rawText, CultureInfo.InvariantCulture),
                                JsonValueKind.String => obj.ToString(), // Directly gets the string.
                                JsonValueKind.True => true,
                                JsonValueKind.False => false,
                                JsonValueKind.Null => null,
                                JsonValueKind.Undefined => null, // Undefined treated as null.
                                JsonValueKind.Object => rawText, // Returns raw JSON for objects.
                                JsonValueKind.Array => rawText, // Returns raw JSON for arrays.
                                _ => rawText // Fallback to raw text for any other kind.
                            };
                        }
                    default:
                        throw new ArgumentException("Expected a JsonElement object", nameof(obj));
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public static Dictionary<string, object> EntityToDictionary(this object ety)
        {
            Type type = ety.GetType();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (System.Reflection.PropertyInfo property in type.GetProperties())
            {
                string propertyName = property.Name;
                object propertyValue = property.GetValue(ety, null);
                if(propertyValue == null)
                    continue;
                // 检查是否为集合类型，如 List<T>
                if (propertyValue is IEnumerable && !(propertyValue is string))
                {
                    // 将集合中的元素转换为数组或其他合适的格式
                    var vals = "";
                    foreach (var val in (IEnumerable)propertyValue)
                    {
                        vals += val + ", ";
                    }
                    vals = vals.TrimEnd(',', ' ');
                    dict.Add(propertyName, vals);
                }
                else
                {
                    // 处理普通属性
                    dict.Add(propertyName, propertyValue);
                }
            }
            return dict;
        }


        public static object ChangeType(this object convertibleValue, Type type)
        {
            if (null == convertibleValue) return null;

            try
            {
                if (type == typeof(Guid) || type == typeof(Guid?))
                {
                    string value = convertibleValue.ToString();
                    if (value == "") return null;
                    return Guid.Parse(value);
                }

                if (!type.IsGenericType) return Convert.ChangeType(convertibleValue, type);
                if (type.ToString() == "System.Nullable`1[System.Boolean]" || type.ToString() == "System.Boolean")
                {
                    if (convertibleValue.ToString() == "0")
                        return false;
                    return true;
                }
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(Nullable<>))
                {
                    return Convert.ChangeType(convertibleValue, Nullable.GetUnderlyingType(type));
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
    }
}
