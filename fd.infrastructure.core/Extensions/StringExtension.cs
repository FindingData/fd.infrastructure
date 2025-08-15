using fd.infrastructure.entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace fd.infrastructure.core.Extensions
{
    public static class StringExtension
    {
        public static bool _windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static string ReplacePath(this string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";
            if (_windows)
                return path.Replace("/", "\\");
            return path.Replace("\\", "/");

        }
        private static DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);

        private static long longTime = 621355968000000000;

        private static int samllTime = 10000000;

        public static bool IsInt(this object obj)
        {
            if (obj == null)
                return false;
            bool reslut = Int32.TryParse(obj.ToString(), out int _number);
            return reslut;

        }
        public static string GetDBCondition(this string stringType)
        {
            string reslut = "";
            switch (stringType?.ToLower())
            {
                case HtmlElementType.droplist:
                case HtmlElementType.selectlist:
                case HtmlElementType.textarea:
                case HtmlElementType.checkbox:
                case HtmlElementType.Contains:
                    reslut = HtmlElementType.Contains;
                    break;
                case HtmlElementType.thanorequal:
                case HtmlElementType.ThanOrEqual:
                    reslut = HtmlElementType.ThanOrEqual;
                    break;
                case HtmlElementType.lessorequal:
                case HtmlElementType.LessOrequal:
                    reslut = HtmlElementType.LessOrequal;
                    break;
                case HtmlElementType.gt:
                case HtmlElementType.GT:
                    reslut = HtmlElementType.GT;
                    break;
                case HtmlElementType.lt:
                case HtmlElementType.LT:
                    reslut = HtmlElementType.lt;
                    break;
                case HtmlElementType.like:
                    reslut = HtmlElementType.like;
                    break;
                default:
                    reslut = HtmlElementType.Equal;
                    break;
            }
            return reslut;
        }

        public static bool IsDate(this object str)
        {
            return str.IsDate(out _);
        }
        public static bool IsDate(this object str, out DateTime dateTime)
        {
            dateTime = DateTime.Now;
            if (str == null || str.ToString() == "")
            {
                return false;
            }
            return DateTime.TryParse(str.ToString(), out dateTime);
        }
        public static bool IsGuid(this string guid)
        {
            Guid newId;
            return guid.GetGuid(out newId);
        }

        public static bool GetGuid(this string guid, out Guid outId)
        {
            Guid emptyId = Guid.Empty;
            return Guid.TryParse(guid, out outId);
        }


        public static int GetInt(this object obj)
        {
            if (obj == null)
                return 0;
            int.TryParse(obj.ToString(), out int _number);
            return _number;

        }

        /// <summary>
        /// 根据传入格式判断是否为小数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="formatString">18,5</param>
        /// <returns></returns>
        public static bool IsNumber(this string str, string formatString)
        {
            if (string.IsNullOrEmpty(str)) return false;

            return Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$");
        }

        public static string CleanGeneratedSql(this string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return string.Empty;

            // 替换 \n, \t 等字符
            return raw
                .Replace("\\n", "\n")
                .Replace("\\t", "\t")
                .Replace("\\r", "")
                .Trim().Trim(';');  // 去掉末尾多余分号
        }



        public static LinqExpressionType GetLinqCondition(this string stringType)
        {
            LinqExpressionType linqExpression;
            switch (stringType)
            {
                case HtmlElementType.Contains:
                    linqExpression = LinqExpressionType.In;
                    break;
                case HtmlElementType.ThanOrEqual:
                    linqExpression = LinqExpressionType.ThanOrEqual;
                    break;
                case HtmlElementType.LessOrequal:
                    linqExpression = LinqExpressionType.LessThanOrEqual;
                    break;
                case HtmlElementType.GT:
                    linqExpression = LinqExpressionType.GreaterThan;
                    break;
                case HtmlElementType.lt:
                    linqExpression = LinqExpressionType.LessThan;
                    break;
                case HtmlElementType.like:
                    linqExpression = LinqExpressionType.Contains;
                    break;
                default:
                    linqExpression = LinqExpressionType.Equal;
                    break;
            }
            return linqExpression;
        }

    }
}
