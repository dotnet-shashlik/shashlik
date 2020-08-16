using Shashlik.Utils.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shashlik.Kernel.Dependency
{
    /// <summary>
    /// 默认使用的约定服务查找器,只会注册接口以及自身为服务
    /// </summary>
    public interface IBasedOnServiceDescriptorProvider
    {
        IEnumerable<ShashlikServiceDescriptor> FromAssembly(Assembly assembly, TypeInfo baseType, ServiceLifetime serviceLifetime);
    }
}
