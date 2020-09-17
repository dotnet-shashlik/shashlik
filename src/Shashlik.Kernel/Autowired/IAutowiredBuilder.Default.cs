using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using Shashlik.Utils.Extensions;
using Microsoft.Extensions.Configuration;

namespace Shashlik.Kernel.Autowired
{
    internal class DefaultAutowiredBuilder : IAutowiredBuilder
    {
        public DefaultAutowiredBuilder(Type autowiredBaseType, IAutowiredProvider autoInitializer,
            DependencyContext dependencyContext)
        {
            AutowiredBaseType = autowiredBaseType;
            AutowiredBaseTypeIsAttribute = autowiredBaseType.IsSubTypeOf<Attribute>();
            AutowiredProvider = autoInitializer;
            DependencyContext = dependencyContext;
        }

        /// <summary>
        /// 自动装配基类
        /// </summary>
        public Type AutowiredBaseType { get; }

        public bool AutowiredBaseTypeIsAttribute { get; }

        public DependencyContext DependencyContext { get; set; }

        /// <summary>
        /// 自动装配初始化器
        /// </summary>
        public IAutowiredProvider AutowiredProvider { get; }
    }


    internal class DefaultAutowiredServiceBuilder : DefaultAutowiredBuilder, IAutowiredServiceBuilder
    {
        public DefaultAutowiredServiceBuilder(Type autowiredBaseType, IAutowiredProvider autoInitializer,
            DependencyContext dependencyContext, IKernelServices kernelService)
            : base(autowiredBaseType, autoInitializer, dependencyContext)
        {
            KernelService = kernelService;
        }

        public IKernelServices KernelService { get; }
    }

    internal class DefaultAutowiredConfigureBuilder : DefaultAutowiredBuilder, IAutowiredConfigureBuilder
    {
        public DefaultAutowiredConfigureBuilder(Type autowiredBaseType, IAutowiredProvider autoInitializer,
            DependencyContext dependencyContext, IKernelConfigure kernelConfigure)
            : base(autowiredBaseType, autoInitializer, dependencyContext)
        {
            KernelConfigure = kernelConfigure;
        }

        public IKernelConfigure KernelConfigure { get; }
    }
}