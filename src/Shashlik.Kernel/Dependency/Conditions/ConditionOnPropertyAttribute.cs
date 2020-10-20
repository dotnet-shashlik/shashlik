using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Shashlik.Utils.Extensions;

namespace Shashlik.Kernel.Dependency.Conditions
{
    /// <summary>
    /// 属性值条件
    /// </summary>
    [ConditionOrder(20)]
    public class ConditionOnPropertyAttribute : Attribute, IConditionBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueType"><paramref name="values"/>值的类型</param>
        /// <param name="property">属性名称</param>
        /// <param name="values">属性值数组,包含关系,只要有一个值相等即认为符合条件</param>
        /// <exception cref="ArgumentException"></exception>
        public ConditionOnPropertyAttribute(Type valueType, string property, params object[] values)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            Property = property.Replace(".", ":");
            Values = values;
        }

        /// <summary>
        /// 属性名称,例:Logging:Enable
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// 值类型
        /// </summary>
        public Type ValueType { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public object[] Values { get; set; }

        /// <summary>
        /// 是否区分大小写,string类型比较时用,bool值的话,到程序中会变成首字母大写,默认不区分true
        /// </summary>
        public bool IgnoreCase { get; set; } = true;

        public bool ConditionOn(IServiceCollection services, IConfiguration rootConfiguration,
            IHostEnvironment hostEnvironment)
        {
            var value = rootConfiguration.GetValue(ValueType, Property);

            var isString = ValueType == typeof(string);
            foreach (var item in Values)
            {
                if (item == value)
                    return true;
                if (item == null || value == null)
                    continue;


                if (isString && (
                    IgnoreCase
                        ? item.ToString().EqualsIgnoreCase(value.ToString())
                        : item.ToString().Equals(value.ToString())
                ))
                    return true;
            }

            return false;
        }
    }
}