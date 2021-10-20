using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;

namespace Shashlik.Sms.Cap.Tests
{
    public class SmsCapTestAutowire: IServiceAssembler
    {
        public void Configure(IKernelServices kernelServices)
        {
            kernelServices.Services.AddMemoryCache();
        }
    }
}