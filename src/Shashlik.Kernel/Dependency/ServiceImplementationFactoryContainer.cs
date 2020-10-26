using System;
using System.Collections.Generic;

namespace Shashlik.Kernel.Dependency
{
    /// <summary>
    /// 服务实现实例工厂方法
    /// </summary>
    internal static class ServiceImplementationFactoryContainer
    {
        public static IDictionary<Type, Func<IServiceProvider, object>> Container { get; } =
            new Dictionary<Type, Func<IServiceProvider, object>>();
    }
}