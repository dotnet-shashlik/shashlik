﻿using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel
{
    /// <summary>
    /// Shashlik 内核 服务配置
    /// </summary>
    public interface IKernelServices
    {
        /// <summary>
        /// 服务集
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// shashlik服务注册与条件集合
        /// </summary>
        List<ShashlikServiceDescriptor> ShashlikServiceDescriptors { get; }

        /// <summary>
        /// 程序集扫描上下文
        /// </summary>
        DependencyContext ScanFromDependencyContext { get; }

        /// <summary>
        /// 根配置
        /// </summary>
        IConfiguration RootConfiguration { get; }
    }
}