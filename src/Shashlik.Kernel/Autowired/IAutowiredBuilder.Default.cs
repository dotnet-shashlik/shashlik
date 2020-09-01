using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using Shashlik.Utils.Extensions;
using Microsoft.Extensions.Configuration;

namespace Shashlik.Kernel.Autowired
{
    class DefaultAutowireBuilder : IAutowiredBuilder
    {
        public DefaultAutowireBuilder(TypeInfo autowireBaseType, IAutowiredProvider autoInitializer,
            DependencyContext dependencyContext)
        {
            AutowireBaseType = autowireBaseType;
            AutowireBaseTypeIsAttribute = autowireBaseType.IsSubTypeOf<Attribute>();
            AutowireProvider = autoInitializer;
            DependencyContext = dependencyContext;
        }

        /// <summary>
        /// 自动装配基类
        /// </summary>
        public TypeInfo AutowireBaseType { get; }

        public bool AutowireBaseTypeIsAttribute { get; }

        public DependencyContext DependencyContext { get; set; }

        /// <summary>
        /// 自动装配初始化器
        /// </summary>
        public IAutowiredProvider AutowireProvider { get; }
    }


    class DefaultAutowiredServiceBuilder : DefaultAutowireBuilder, IAutowiredServiceBuilder
    {
        public DefaultAutowiredServiceBuilder(TypeInfo autowireBaseType, IAutowiredProvider autoInitializer,
            DependencyContext dependencyContext, IKernelServices kernelService)
            : base(autowireBaseType, autoInitializer, dependencyContext)
        {
            KernelService = kernelService;
        }

        public IKernelServices KernelService { get; }
    }

    class DefaultAutowiredConfigureBuilder : DefaultAutowireBuilder, IAutowiredConfigureBuilder
    {
        public DefaultAutowiredConfigureBuilder(TypeInfo autowireBaseType, IAutowiredProvider autoInitializer,
            DependencyContext dependencyContext, IKernelConfigure kernelConfigure)
            : base(autowireBaseType, autoInitializer, dependencyContext)
        {
            KernelConfigure = kernelConfigure;
        }

        public IKernelConfigure KernelConfigure { get; }
    }
}