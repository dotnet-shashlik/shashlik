﻿using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Shashlik.Utils.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Shashlik.Kernel.Autowired
{
    /// <summary>
    /// 自动装配构建
    /// </summary>
    public interface IAutowiredBuilder
    {
        /// <summary>
        /// 自动装配类型
        /// </summary>
        Type AutowiredBaseType { get; }

        /// <summary>
        /// 装配类型是否为特性
        /// </summary>
        bool AutowiredBaseTypeIsAttribute { get; }

        // /// <summary>
        // /// 需要移除的装配类型
        // /// </summary>
        // HashSet<TypeInfo> Removes { get; }

        /// <summary>
        /// 扫描依赖上下文
        /// </summary>
        DependencyContext DependencyContext { get; }

        /// <summary>
        /// 自动装配提供类
        /// </summary>
        IAutowiredProvider AutowiredProvider { get; }
    }

    public interface IAutowiredServiceBuilder : IAutowiredBuilder
    {
        IKernelServices KernelService { get; }
    }

    public interface IAutowiredConfigureBuilder : IAutowiredBuilder
    {
        IKernelConfigure KernelConfigure { get; }
    }
}