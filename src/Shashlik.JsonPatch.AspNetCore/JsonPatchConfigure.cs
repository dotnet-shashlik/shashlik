using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;

namespace Shashlik.JsonPatch.AspNetCore
{
    public class JsonPatchConfigure : IAutowiredConfigureServices
    {
        public void ConfigureServices(IKernelServices kernelService)
        {
            var builder = kernelService.Services.AddControllers()
                .AddNewtonsoftJson();
        }
    }
}