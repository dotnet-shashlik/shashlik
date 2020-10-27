using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shashlik.Utils.Extensions;

namespace Shashlik.Kernel.Attributes
{
    /// <summary>
    /// 属性值条件,优先级20
    /// </summary>
    [Order(20)]
    public class ConditionOnPropertyAttribute : Attribute, IConditionBase
    {
        /// <summary>
        /// 属性值条件,优先级20
        /// </summary>
        /// <param name="valueType"><paramref name="value"/>值的类型</param>
        /// <param name="property">属性名称</param>
        /// <param name="value">属性值</param>
        /// <exception cref="ArgumentException"></exception>
        public ConditionOnPropertyAttribute(Type valueType, string property, object value)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            Property = property.Replace(".", ":");
            Value = value;
        }

        /// <summary>
        /// 属性名称,例:Logging:Enable
        /// </summary>
        public string Property { get; }

        /// <summary>
        /// 值类型
        /// </summary>
        public Type ValueType { get; }

        /// <summary>
        /// 默认值,当不存在该属性值时
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// 是否区分大小写,string类型比较时用
        /// </summary>
        public bool IgnoreCase { get; set; } = true;

        public bool ConditionOn(IServiceCollection services, IConfiguration rootConfiguration,
            IHostEnvironment hostEnvironment)
        {
            var value = rootConfiguration.GetValue(ValueType, Property, DefaultValue);

            var isString = ValueType == typeof(string);
            if (Value != null && Value.Equals(value))
                return true;
            if (Value == null && value == null)
                return true;
            if (Value == null || value == null)
                return false;

            return isString && (
                IgnoreCase
                    ? Value.ToString().EqualsIgnoreCase(value.ToString())
                    : Value.ToString().Equals(value.ToString())
            );
        }
    }
}