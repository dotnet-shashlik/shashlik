using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

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

        public bool ConditionOn(IServiceCollection services, IConfiguration rootConfiguration, IHostEnvironment hostEnvironment)
        {
            var value = rootConfiguration[Property];
            return (value == null && MatchIfMissing) || Value == value;
        }
    }
}
