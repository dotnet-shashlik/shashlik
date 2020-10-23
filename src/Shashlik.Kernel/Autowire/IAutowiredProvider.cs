using System;
using System.Collections.Generic;

namespace Shashlik.Kernel.Autowire
{
    /// <summary>
    /// 自动装配初始化器
    /// </summary>
    public interface IAutowireProvider<T> where T : IAutowire
    {
        /// <summary>
        /// 执行装配
        /// </summary>
        /// <param name="autowiredService"></param>
        /// <param name="autowiredAction"></param>
        void Autowire(IDictionary<Type, AutowireDescriptor<T>> autowiredService,
            Action<AutowireDescriptor<T>> autowiredAction);

        /// <summary>
        /// 从服务提供类加载
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        IDictionary<Type, AutowireDescriptor<T>> Load(IKernelServices kernelServices,
            IServiceProvider serviceProvider);
    }
}