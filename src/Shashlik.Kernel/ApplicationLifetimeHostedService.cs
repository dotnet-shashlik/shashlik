using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Shashlik.Kernel.Assembler;

namespace Shashlik.Kernel
{
    /// <summary>
    /// 应用程序生命周期钩子
    /// </summary>
    public class ApplicationLifetimeHostedService : IHostedService
    {
        public ApplicationLifetimeHostedService(
            IAssemblerProvider<IApplicationStartAssembler> applicationStartAutowireProvider,
            IAssemblerProvider<IApplicationStopAssembler> applicationStopAutowireProvider,
            IKernelServices kernelServices,
            IServiceProvider serviceProvider)
        {
            ApplicationStartAutowireProvider = applicationStartAutowireProvider;
            ApplicationStopAutowireProvider = applicationStopAutowireProvider;
            KernelServices = kernelServices;
            ServiceProvider = serviceProvider;
        }

        private IServiceProvider ServiceProvider { get; }
        private IAssemblerProvider<IApplicationStartAssembler> ApplicationStartAutowireProvider { get; }
        private IAssemblerProvider<IApplicationStopAssembler> ApplicationStopAutowireProvider { get; }
        private IKernelServices KernelServices { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var dic = ApplicationStartAutowireProvider.Load(KernelServices, ServiceProvider);
            async void AutowiredAction(AssemblerDescriptor<IApplicationStartAssembler> r) => await r.ServiceInstance.OnStart(cancellationToken);
            ApplicationStartAutowireProvider.Execute(dic,
                AutowiredAction);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            var dic = ApplicationStopAutowireProvider.Load(KernelServices, ServiceProvider);
            async void AutowiredAction(AssemblerDescriptor<IApplicationStopAssembler> r) => await r.ServiceInstance.OnStop(cancellationToken);
            ApplicationStopAutowireProvider.Execute(dic,
                AutowiredAction);
            return Task.CompletedTask;
        }
    }
}