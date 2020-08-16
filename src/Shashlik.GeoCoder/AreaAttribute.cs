using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.GeoCoder
{
    /// <summary>
    /// 区域验证
    /// </summary>
    public class AreaAttribute : ValidationAttribute
    {
        /// <summary>
        /// 区域验证
        /// </summary>
        /// <param name="level">最低选到哪一级</param>
        public AreaAttribute(AreaLevel level)
        {
            Level = level;
        }

        /// <summary>
        /// 最低选到哪一级
        /// </summary>
        public AreaLevel Level { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is AreaModel))
                return ValidationResult.Success;

            var area = value as AreaModel;
            if (area == null)
                return ValidationResult.Success;
            var areaService = validationContext.GetRequiredService<IAreaService>();

            if (areaService.IsValid(area, Level, out string msg))
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage ?? msg);
        }
    }
}