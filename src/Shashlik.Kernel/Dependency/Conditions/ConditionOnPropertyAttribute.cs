using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Shashlik.Utils.Extensions;

namespace Shashlik.Kernel.Dependency.Conditions
{
    /// <summary>
    /// 属性值条件
    /// </summary>
    [ConditionOrder(20)]
    public class ConditionOnPropertyAttribute : Attribute, IConditionBase
    {
        public ConditionOnPropertyAttribute(string property, string value, bool matchIfMissing = false)
        {
            if (string.IsNullOrWhiteSpace(property))
            {
                throw new ArgumentException($"“{nameof(property)}”不能为 Null 或空白", nameof(property));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"“{nameof(value)}”不能为 Null 或空白", nameof(value));
            }

            Property = property;
            Value = value;
            MatchIfMissing = matchIfMissing;
        }

        /// <summary>
        /// 属性名称,例:Logging:Enable
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 未配置时是否注册
        /// </summary>
        public bool MatchIfMissing { get; set; } = false;

        /// <summary>
        /// 是否区分大小写,bool值的话,到程序中会变成首字母大写,默认不区分true
        /// </summary>
        public bool IgnoreCase { get; set; } = true;

        public bool ConditionOn(IServiceCollection services, IConfiguration rootConfiguration,
            IHostEnvironment hostEnvironment)
        {
            var value = rootConfiguration.GetValue<string>(Property);
            return value switch
            {
                null when MatchIfMissing => true,
                null => false,
                _ => IgnoreCase ? value.EqualsIgnoreCase(Value) : value.Equals(Value)
            };
        }
    }
}