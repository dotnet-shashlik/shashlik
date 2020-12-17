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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ConditionOnHostEnvironmentAttribute : ConditionBaseAttribute
    {
        /// <summary>
        /// 环境名称判断,优先级0
        /// </summary>
        /// <param name="envName"></param>
        /// <exception cref="ArgumentException"></exception>
        public ConditionOnHostEnvironmentAttribute(string envName)
        {
            if (string.IsNullOrWhiteSpace(envName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(envName));
            EnvName = envName;
        }

        public string EnvName { get; }

        public bool IgnoreCase { get; set; } = true;

        public override bool ConditionOn(
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