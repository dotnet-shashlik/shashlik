using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.AspNetCore.Middlewares;
using Shashlik.AspNetCore.PatchUpdate;
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
            var builder = kernelService.Services.AddControllers(options =>
            {
                if (Options.UsePatchUpdateBinder)
                    options.ModelBinderProviders.Insert(0, new PatchUpdateBinderProvider());
            });

            if (Options.UseNewtonsoftJson)
                builder.AddNewtonsoftJson();
        }
    }
}