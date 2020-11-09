using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shashlik.Utils.Extensions;

namespace Shashlik.Kernel.Attributes
{
    /// <summary>
    /// 环境名称判断,优先级0, 自动装配类IServiceAutowire无效
    /// </summary>
    [Order(0)]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ConditionOnHostEnvironmentAttribute : Attribute, IConditionBase
    {
        /// <summary>
        /// 环境名称判断,优先级0
        /// </summary>
        /// <param name="envName"></param>
        /// <exception cref="ArgumentException"></exception>
        public ConditionOnHostEnvironmentAttribute(string envName)
        {
            if (string.IsNullOrWhiteSpace(envName))
            {
                throw new ArgumentException($"“{nameof(envName)}”不能为 Null 或空白", nameof(envName));
            }

            EnvName = envName;
        }

        public string EnvName { get; }

        public bool IgnoreCase { get; set; } = true;

        public bool ConditionOn(
            IServiceCollection services,
            ServiceDescriptor serviceDescriptor,
            IConfiguration rootConfiguration,
            IHostEnvironment hostEnvironment)
        {
            return IgnoreCase
                ? hostEnvironment.EnvironmentName.EqualsIgnoreCase(EnvName)
                : hostEnvironment.EnvironmentName.Equals(EnvName);
        }
    }
}