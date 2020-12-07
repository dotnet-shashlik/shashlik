using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shashlik.Utils.Extensions;

namespace Shashlik.Kernel.Attributes
{
    /// <summary>
    /// 属性值条件,优先级20, 自动装配类IServiceAutowire无效
    /// </summary>
    [Order(20)]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ConditionOnPropertyAttribute : ConditionBaseAttribute
    {
        /// <summary>
        /// 属性值条件,优先级20
        /// </summary>
        /// <param name="valueType"><paramref name="value"/>值的类型</param>
        /// <param name="property">属性名称</param>
        /// <param name="value">属性值</param>
        /// <param name="supportDot">是否支持小数点作为连接符</param>
        /// <exception cref="ArgumentException"></exception>
        public ConditionOnPropertyAttribute(Type valueType, string property, object? value, bool supportDot = true)
        {
            if (property is null) throw new ArgumentNullException(nameof(property));
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            Property = supportDot ? property.Replace(".", ":") : property;
            Value = value;
            SupportDot = supportDot;
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
        public object? DefaultValue { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public object? Value { get; }

        /// <summary>
        /// 是否区分大小写,string类型比较时用
        /// </summary>
        public bool IgnoreCase { get; set; } = true;

        /// <summary>
        /// 是否支持小数点作为路径连接符,默认true
        /// </summary>
        public bool SupportDot { get; }

        public override bool ConditionOn(
            IServiceCollection services,
            ServiceDescriptor serviceDescriptor,
            IConfiguration rootConfiguration,
            IHostEnvironment hostEnvironment)
        {
            var proValue = rootConfiguration.GetValue(ValueType, Property, DefaultValue);

            if (Value is null && proValue is null)
                return true;
            if (Value is null || proValue is null)
                return false;
            if (Value.Equals(proValue))
                return true;
            var isString = ValueType == typeof(string);

            return isString && (
                IgnoreCase
                    ? Value.ToString().EqualsIgnoreCase(proValue.ToString())
                    : Value.ToString().Equals(proValue.ToString())
            );
        }
    }
}