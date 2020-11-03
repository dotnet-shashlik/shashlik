using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Shashlik.Kernel.Autowire;

namespace Shashlik.Kernel
{
    /// <summary>
    /// 应用程序生命周期钩子
    /// </summary>
    public class ApplicationLifetimeHostedService : IHostedService
    {
        public ApplicationLifetimeHostedService(
            IAutowireProvider<IApplicationStartAutowire> applicationStartAutowireProvider,
            IAutowireProvider<IApplicationStopAutowire> applicationStopAutowireProvider,
            IKernelServices kernelServices,
            IServiceProvider serviceProvider)
        {
            ApplicationStartAutowireProvider = applicationStartAutowireProvider;
            ApplicationStopAutowireProvider = applicationStopAutowireProvider;
            KernelServices = kernelServices;
            ServiceProvider = serviceProvider;
        }

        private IServiceProvider ServiceProvider { get; }
        private IAutowireProvider<IApplicationStartAutowire> ApplicationStartAutowireProvider { get; }
        private IAutowireProvider<IApplicationStopAutowire> ApplicationStopAutowireProvider { get; }
        private IKernelServices KernelServices { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var dic = ApplicationStartAutowireProvider.Load(KernelServices, ServiceProvider);
            ApplicationStartAutowireProvider.Autowire(dic,
                async r => await r.ServiceInstance.OnStart(cancellationToken));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            var dic = ApplicationStopAutowireProvider.Load(KernelServices, ServiceProvider);

            ApplicationStopAutowireProvider.Autowire(dic,
                async r => await r.ServiceInstance.OnStop(cancellationToken));
            return Task.CompletedTask;
        }
    }
}