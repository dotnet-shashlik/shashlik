using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;

namespace Shashlik.Sms.Cap.Tests
{
    public class SmsCapTestAutowire: IServiceAutowire
    {
        public void Configure(IKernelServices kernelServices)
        {
            kernelServices.Services.AddMemoryCache();
        }
    }
}