using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddShashlik(Configuration)
                .AutowiredOptions()
                .AutowiredServices()
                .BeginAutowired<ITestAutowiredServices>()
                .Build(r => (r.ServiceInstance as ITestAutowiredServices)!.ConfigureServices(services))
                .AddServicesByBasedOn<ITestBasedOn>(ServiceLifetime.Singleton);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.ApplicationServices.UseShashlik()
                .AutowiredConfigure()
                .AutowiredAspNetConfigure(app)
                .BeginAutowired<ITestAutowiredConfigure>()
                .Build(r => (r.ServiceInstance as ITestAutowiredConfigure)!.Configure(app.ApplicationServices));
        }
    }
}