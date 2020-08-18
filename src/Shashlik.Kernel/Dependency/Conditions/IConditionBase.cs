using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Kernel.Dependency.Conditions
{
    /// <summary>
    /// 条件特性基础接口,可以任意扩展
    /// </summary>
    public interface IConditionBase
    {
        /// <summary>
        /// 条件注册,满足条件才会注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="rootConfiguration"></param>
        /// <returns></returns>
        bool ConditionOn(IServiceCollection services, IConfiguration rootConfiguration, IHostEnvironment hostEnvironment);
    }
}
