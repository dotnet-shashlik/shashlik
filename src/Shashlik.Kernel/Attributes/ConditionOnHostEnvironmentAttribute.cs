using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shashlik.Utils.Extensions;

namespace Shashlik.Kernel.Attributes
{
    /// <summary>
    /// 环境名称判断
    /// </summary>
    [Order(0)]
    public class ConditionOnHostEnvironmentAttribute : Attribute, IConditionBase
    {
        public ConditionOnHostEnvironmentAttribute(string envName)
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
