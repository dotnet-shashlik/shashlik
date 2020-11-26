using System;

namespace Shashlik.Kernel
{
    /// <summary>
    /// Shashlik服务提供类,全局静态类
    /// </summary>
    public static class GlobalKernelServiceProvider
    {
        public static IKernelServiceProvider? KernelServiceProvider { get; private set; }

        internal static void InitServiceProvider(IKernelServiceProvider serviceProvider)
        {
            if (KernelServiceProvider != null)
                throw new InvalidOperationException("Kernel ServiceProvider cannot repeat initialization");

            KernelServiceProvider = serviceProvider;
        }
    }
}