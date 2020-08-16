using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Shashlik.Utils.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Shashlik.Kernel.Autowire
{
    /// <summary>
    /// 自动装配构建
    /// </summary>
    public interface IAutowireBuilder
    {
        /// <summary>
        /// 自动装配类型
        /// </summary>
        TypeInfo AutowireBaseType { get; }

        /// <summary>
        /// 装配类型是否为特性
        /// </summary>
        bool AutowireBaseTypeIsAttribute { get; }

        /// <summary>
        /// 需要替换的装配类型
        /// </summary>
        IDictionary<TypeInfo, TypeInfo> Replaces { get; }

        /// <summary>
        /// 需要移除的装配类型
        /// </summary>
        HashSet<TypeInfo> Removes { get; }

        /// <summary>
        /// 扫描依赖上下文
        /// </summary>
        DependencyContext DependencyContext { get; set; }

        /// <summary>
        /// 自动装配初始化器
        /// </summary>
        IAutoInitializer AutoInitializer { get; }
    }

    public interface IAutowireServiceBuilder : IAutowireBuilder
    {
        IKernelService KernelService { get; }
    }

    public interface IAutowireConfigureBuilder : IAutowireBuilder
    {
        IKernelConfigure KernelConfigure { get; }
    }
}
