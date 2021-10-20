using System;
using System.Collections.Generic;

namespace Shashlik.Kernel.Options
{
    public interface IOptionsAssembler
    {
        /// <summary>
        /// 配置options
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <param name="disabledAutoOptionTypes"></param>
        void ConfigureAll(IKernelServices kernelServices, IEnumerable<Type> disabledAutoOptionTypes);
    }
}