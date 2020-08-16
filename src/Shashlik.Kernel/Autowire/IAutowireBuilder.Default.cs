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
        public DefaultAutowireBuilder(TypeInfo autowireBaseType, IAutoInitializer autoInitializer)
        {
            AutowireBaseType = autowireBaseType;
            AutowireBaseTypeIsAttribute = autowireBaseType.IsChildTypeOf<Attribute>();
            AutoInitializer = autoInitializer;
        }

        /// <summary>
        /// 自动装配基类
        /// </summary>
        public TypeInfo AutowireBaseType { get; }

        public bool AutowireBaseTypeIsAttribute { get; }

        public IDictionary<TypeInfo, TypeInfo> Replaces { get; } = new Dictionary<TypeInfo, TypeInfo>();

        public HashSet<TypeInfo> Removes { get; } = new HashSet<TypeInfo>();

        public DependencyContext DependencyContext { get; set; }

        /// <summary>
        /// 自动装配初始化器
        /// </summary>
        public IAutoInitializer AutoInitializer { get; }
    }


    class DefaultAutowireServiceBuilder : DefaultAutowireBuilder, IAutowireServiceBuilder
    {
        public DefaultAutowireServiceBuilder(TypeInfo autowireBaseType, IAutoInitializer autoInitializer, IKernelService kernelService)
            : base(autowireBaseType, autoInitializer)
        {
            KernelService = kernelService;
        }

        public IKernelService KernelService { get; }
    }

    class DefaultAutowireConfigureBuilder : DefaultAutowireBuilder, IAutowireConfigureBuilder
    {

        public DefaultAutowireConfigureBuilder(TypeInfo autowireBaseType, IAutoInitializer autoInitializer, IKernelConfigure kernelConfigure)
            : base(autowireBaseType, autoInitializer)
        {
            KernelConfigure = kernelConfigure;
        }

        public IKernelConfigure KernelConfigure { get; }
    }
}
