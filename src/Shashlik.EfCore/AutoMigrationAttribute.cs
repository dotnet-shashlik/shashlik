using System;
using Microsoft.Extensions.Configuration;
using Shashlik.Utils.Extensions;

// ReSharper disable ClassNeverInstantiated.Global

namespace Shashlik.EfCore
{
    /// <summary>
    /// 允许自动迁移
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AutoMigrationAttribute : Attribute
    {
        public AutoMigrationAttribute(bool enableAutoMigration = true)
        {
            EnableAutoMigration = enableAutoMigration;
        }

        /// <summary>
        /// 从配置文件中获取条件来确认是否执行自动迁移
        /// </summary>
        /// <param name="valueType">值类型</param>
        /// <param name="property">属性名称</param>
        /// <param name="value">条件值</param>
        /// <param name="defaultValue">默认值,配置不存在时</param>
        /// <param name="ignoreCase">valueType为字符串时,是否忽略大小写</param>
        /// <exception cref="ArgumentException"></exception>
        public AutoMigrationAttribute(Type valueType, string property, object value,
            object? defaultValue = null, bool ignoreCase = true)
        {
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            if (string.IsNullOrWhiteSpace(property))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(property));

            Property = property.Replace(".", ":");
            Value = value;
            DefaultValue = defaultValue;
            IgnoreCase = ignoreCase;
        }

        /// <summary>
        /// 是否执行自动迁移
        /// </summary>
        public bool EnableAutoMigration { get; }

        public Type? ValueType { get; }

        public string? Property { get; }

        public object? Value { get; }

        public object? DefaultValue { get; }

        /// <summary>
        /// 是否区分大小写,string类型比较时用
        /// </summary>
        public bool IgnoreCase { get; }

        public bool GetEnableAutoMigration(IConfiguration? configuration)
        {
            if (EnableAutoMigration) return true;


            var value = configuration.GetValue(ValueType, Property, DefaultValue);

            var isString = ValueType == typeof(string);
            if (Value != null && Value.Equals(value))
                return true;
            if (Value is null && value is null)
                return true;
            if (Value is null || value is null)
                return false;

            return isString && (
                IgnoreCase
                    ? Value.ToString().EqualsIgnoreCase(value.ToString())
                    : Value.ToString().Equals(value.ToString())
            );
        }
    }
}