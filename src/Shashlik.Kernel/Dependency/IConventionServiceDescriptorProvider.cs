using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Shashlik.Kernel.Dependency
{
    /// <summary>
    /// 服务提供器
    /// </summary>
    public interface IConventionServiceDescriptorProvider
    {
        /// <summary>
        /// 从程序集中查找服务
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <returns></returns>
        IEnumerable<ShashlikServiceDescriptor> FromAssembly(Assembly assembly);
    }
}