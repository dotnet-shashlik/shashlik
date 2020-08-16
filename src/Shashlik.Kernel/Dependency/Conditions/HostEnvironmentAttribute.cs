using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shashlik.Utils.Extensions;
using System;

namespace Shashlik.Kernel.Dependency.Conditions
{
    /// <summary>
    /// 环境变了判断
    /// </summary>
    [ConditionOrder(0)]
    public class HostEnvironmentAttribute : Attribute, IConditionBase
    {
        public HostEnvironmentAttribute(string envName)
        {
            if (string.IsNullOrWhiteSpace(envName))
            {
                throw new ArgumentException($"“{nameof(envName)}”不能为 Null 或空白", nameof(envName));
            }

            EnvName = envName;
        }

        public string EnvName { get; set; }

        public bool IgnoreCase { get; set; } = true;

        public bool ConditionOn(IServiceCollection services, IConfiguration rootConfiguration, IHostEnvironment hostEnvironment)
        {
            if (IgnoreCase)
                return hostEnvironment.EnvironmentName.EqualsIgnoreCase(EnvName);
            return hostEnvironment.EnvironmentName.Equals(EnvName);
        }
    }
}
