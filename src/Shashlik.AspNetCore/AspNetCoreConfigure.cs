using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;

namespace Shashlik.AspNetCore
{
    public class AspNetCoreConfigure : IAutowiredConfigureServices
    {
        public AspNetCoreConfigure(IOptions<AspNetCoreOptions> options)
        {
            Options = options.Value;
        }

        private AspNetCoreOptions Options { get; }

        public void ConfigureServices(IKernelServices kernelService)
        {
            var builder = kernelService.Services.AddControllers();
            if (Options.UseNewtonsoftJson)
                builder.AddNewtonsoftJson();
        }
    }
}