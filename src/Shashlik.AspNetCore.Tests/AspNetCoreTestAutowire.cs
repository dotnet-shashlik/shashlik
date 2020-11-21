using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;

namespace Shashlik.AspNetCore.Tests
{
    public class AspNetCoreTestAutowire : IServiceAutowire
    {
        public void Configure(IKernelServices kernelServices)
        {
            kernelServices.Services.AddControllers()
                .AddApplicationPart(this.GetType().Assembly)
                .AddControllersAsServices();
        }
    }
}