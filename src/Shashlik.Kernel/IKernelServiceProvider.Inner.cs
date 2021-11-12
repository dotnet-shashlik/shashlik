using System;

namespace Shashlik.Kernel
{
    internal class InnerKernelServiceProvider : IKernelServiceProvider
    {
        public InnerKernelServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        private IServiceProvider ServiceProvider { get; }
        public object? GetService(Type serviceType) => ServiceProvider!.GetService(serviceType);
    }
}