using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Kernel
{
    /// <summary>
    /// Shashlik kernel 配置
    /// </summary>
    public interface IKernelConfigure
    {
        IServiceProvider ServiceProvider { get; }
    }

    class KernelConfigure : IKernelConfigure
    {
        public KernelConfigure(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }
    }
}
