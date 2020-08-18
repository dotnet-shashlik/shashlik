using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Shashlik.Kernel
{
    /// <summary>
    /// Shashlik 内核 服务配置
    /// </summary>
    public interface IKernelService
    {
        /// <summary>
        /// 服务集
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// 程序集扫描上下文
        /// </summary>
        DependencyContext ScanFromDependencyContext { get; }

        /// <summary>
        /// 根配置
        /// </summary>
        IConfiguration RootConfiguration { get; }
    }

    class KernelService : IKernelService
    {
        public KernelService(
            IServiceCollection services,
            DependencyContext scanFromDependencyContext,
            IConfiguration rootConfiguration)
        {
            Services = services;
            ScanFromDependencyContext = scanFromDependencyContext;
            RootConfiguration = rootConfiguration;
        }

        public IServiceCollection Services { get; }

        public DependencyContext ScanFromDependencyContext { get; }

        public IConfiguration RootConfiguration { get; }
    }
}
