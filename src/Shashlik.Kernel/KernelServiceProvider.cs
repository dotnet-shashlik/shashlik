using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Kernel
{
    /// <summary>
    /// Shashlik服务提供类,全局静态类
    /// </summary>
    public class KernelServiceProvider
    {
        /// <summary>
        /// Shashlik服务提供类,root
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; }

        internal static void InitServiceProvider(IServiceProvider serviceProvider)
        {
            if (ServiceProvider != null)
                throw new InvalidOperationException("Kernel ServiceProvider cannot repeat initialization.");

            ServiceProvider = serviceProvider;
        }
    }
}
