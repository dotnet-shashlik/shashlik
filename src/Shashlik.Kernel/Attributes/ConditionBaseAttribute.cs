using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shashlik.Kernel.Attributes
{
    /// <summary>
    /// 条件特性基础接口,可以任意扩展
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class ConditionBaseAttribute : Attribute
    {
        /// <summary>
        /// 条件注册,满足条件才会注册
        /// </summary>
        /// <param name="services">服务</param>
        /// <param name="serviceDescriptor">服务描述类</param>
        /// <param name="rootConfiguration">根配置</param>
        /// <param name="hostEnvironment">环境变量</param>
        /// <returns></returns>
        public abstract bool ConditionOn(
            IServiceCollection services,
            ServiceDescriptor serviceDescriptor,
            IConfiguration rootConfiguration,
            IHostEnvironment hostEnvironment);
    }
}