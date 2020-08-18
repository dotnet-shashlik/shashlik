using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using Shashlik.Utils.Extensions;
using Microsoft.Extensions.Configuration;

namespace Shashlik.Kernel.Autowire
{
    class DefaultAutowireBuilder : IAutowireBuilder
    {
        public DefaultAutowireBuilder(TypeInfo autowireBaseType, IAutowireProvider autoInitializer,
            DependencyContext dependencyContext, IConfiguration rootConfiguration)
        {
            AutowireBaseType = autowireBaseType;
            AutowireBaseTypeIsAttribute = autowireBaseType.IsSubTypeOf<Attribute>();
            AutowireProvider = autoInitializer;
            DependencyContext = dependencyContext;
            RootConfiguration = rootConfiguration;
        }

        /// <summary>
        /// 自动装配基类
        /// </summary>
        public TypeInfo AutowireBaseType { get; }

        public bool AutowireBaseTypeIsAttribute { get; }

        public HashSet<TypeInfo> Removes { get; } = new HashSet<TypeInfo>();

        public DependencyContext DependencyContext { get; set; }

        /// <summary>
        /// 自动装配初始化器
        /// </summary>
        public IAutowireProvider AutowireProvider { get; }

        public IConfiguration RootConfiguration { get; set; }
    }


    class DefaultAutowireServiceBuilder : DefaultAutowireBuilder, IAutowireServiceBuilder
    {
        public DefaultAutowireServiceBuilder(TypeInfo autowireBaseType, IAutowireProvider autoInitializer,
            DependencyContext dependencyContext, IConfiguration rootConfiguration, IKernelServices kernelService)
            : base(autowireBaseType, autoInitializer, dependencyContext, rootConfiguration)
        {
            KernelService = kernelService;
        }

        public IKernelServices KernelService { get; }
    }

    class DefaultAutowireConfigureBuilder : DefaultAutowireBuilder, IAutowireConfigureBuilder
    {

        public DefaultAutowireConfigureBuilder(TypeInfo autowireBaseType, IAutowireProvider autoInitializer,
            DependencyContext dependencyContext, IConfiguration rootConfiguration, IKernelConfigure kernelConfigure)
           : base(autowireBaseType, autoInitializer, dependencyContext, rootConfiguration)
        {
            KernelConfigure = kernelConfigure;
        }

        public IKernelConfigure KernelConfigure { get; }
    }
}
