using System;
using System.Collections.Generic;

namespace Shashlik.Kernel.Assembler
{
    /// <summary>
    /// 自动装配初始化器
    /// </summary>
    public interface IAssemblerProvider<T> where T : IAssembler
    {
        /// <summary>
        /// 执行装配
        /// </summary>
        /// <param name="autowiredService"></param>
        /// <param name="autowiredAction"></param>
        void Execute(IDictionary<Type, AssemblerDescriptor<T>> autowiredService,
            Action<AssemblerDescriptor<T>> autowiredAction);

        /// <summary>
        /// 从服务提供类加载
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        IDictionary<Type, AssemblerDescriptor<T>> Load(IKernelServices kernelServices,
            IServiceProvider serviceProvider);
    }
}