using System.Linq;
using AspectCore.Configuration;
using AspectCore.DynamicProxy;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.AspNetCore;
using Shashlik.Kernel.Autowired;
using Shashlik.Kernel.Test.Autowired;
using Shashlik.Kernel.Test.Options;
using Shashlik.Kernel.Test.TestClasses;

namespace Shashlik.Kernel.Test
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.Configure<TestOptions2>(r => r.Enable = false);

            services.AddShashlikCore(Configuration)
                .AutowireOptions()
                .RegistryConventionServices()
                .AddServicesByBasedOn<ITestBasedOn>(ServiceLifetime.Singleton)
                .AutowireServices()
                .Autowire<ITestAutowiredServices>(r => r.ConfigureServices(services))
                .DoFilter();

            services.ConfigureDynamicProxy(r =>
            {
                r.Interceptors.AddTyped<CustomInterceptorAttribute>(
                    p => p.GetCustomAttributes(typeof(CustomInterceptorAttribute), true).Any()
                );

                r.Interceptors.AddTyped<CustomInterceptor2Attribute>(
                    p => p.GetCustomAttributes(typeof(CustomInterceptor2Attribute), true).Any()
                );
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.ApplicationServices.UseShashlik()
                .AutowireServiceProvider()
                .AutowireAspNet(app)
                .Autowire<ITestAutowiredConfigure>(r => r.Configure(app.ApplicationServices))
                ;
        }
    }
}