using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;

namespace Shashlik.Captcha.Tests
{
    public class Configure :IAutowiredConfigureServices
    {
        public void ConfigureServices(IKernelServices kernelService)
        {
            kernelService.Services.AddDataProtection();
        }
    }
}