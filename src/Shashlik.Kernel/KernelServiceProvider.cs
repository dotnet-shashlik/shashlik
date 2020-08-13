using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.Kernel
{
    /// <summary>
    /// Guc服务提供类
    /// </summary>
    public class KernelServiceProvider
    {
        /// <summary>
        /// Guc服务提供类
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; }

        internal static void InitServiceProvider(IServiceProvider serviceProvider)
        {
            if (ServiceProvider != null)
                throw new InvalidOperationException("Kernel ServiceProvider can't repeat initialization.");

            ServiceProvider = serviceProvider;
        }
    }
}
