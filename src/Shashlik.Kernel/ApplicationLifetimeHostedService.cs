using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shashlik.Kernel.Autowire;

namespace Shashlik.Kernel
{
    /// <summary>
    /// 应用程序生命周期钩子
    /// </summary>
    public class ApplicationLifetimeHostedService : IHostedService
    {
        public ApplicationLifetimeHostedService(IServiceScopeFactory serviceScopeFactory,
            IAutowireProvider<IApplicationStartAutowire> applicationStartAutowireProvider,
            IAutowireProvider<IApplicationStopAutowire> applicationStopAutowireProvider, IKernelServices kernelServices)
        {
            ServiceScopeFactory = serviceScopeFactory;
            ApplicationStartAutowireProvider = applicationStartAutowireProvider;
            ApplicationStopAutowireProvider = applicationStopAutowireProvider;
            KernelServices = kernelServices;
        }

        private IServiceScopeFactory ServiceScopeFactory { get; }
        private IAutowireProvider<IApplicationStartAutowire> ApplicationStartAutowireProvider { get; }
        private IAutowireProvider<IApplicationStopAutowire> ApplicationStopAutowireProvider { get; }
        private IKernelServices KernelServices { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = ServiceScopeFactory.CreateScope();

            var dic = ApplicationStartAutowireProvider.Load(KernelServices, scope.ServiceProvider);

            ApplicationStartAutowireProvider.Autowire(dic,
                async r => await r.ServiceInstance.OnStart(cancellationToken));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = ServiceScopeFactory.CreateScope();

            var dic = ApplicationStopAutowireProvider.Load(KernelServices, scope.ServiceProvider);

            ApplicationStopAutowireProvider.Autowire(dic,
                async r => await r.ServiceInstance.OnStop(cancellationToken));
            return Task.CompletedTask;
        }
    }
}