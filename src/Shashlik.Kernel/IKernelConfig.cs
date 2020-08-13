using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.Kernel
{
    /// <summary>
    /// guc kernel 配置
    /// </summary>
    public interface IKernelConfig
    {
        IServiceProvider ServiceProvider { get; }
    }

    class KernelConfig : IKernelConfig
    {
        public KernelConfig(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }
    }
}
