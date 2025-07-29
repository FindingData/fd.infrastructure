using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.entity
{
    public class PlatformTypeAttribute : ValidationAttribute
    {
        private static readonly HashSet<string> ValidPlatforms = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "iOS", "Android"
            };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var str = value?.ToString();
            if (!string.IsNullOrEmpty(str) && ValidPlatforms.Contains(str))
                return ValidationResult.Success;

            return new ValidationResult("平台类型必须是 iOS 或 Android");
        }
    }
}
