using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Kernel.Test.Autowired.TestAutowireConditionClasses
{
    public class FailConditionAutowire : IServiceAutowire
    {
        public void Configure(IKernelServices kernelServices)
        {
            kernelServices.Services.AddSingleton(typeof(FailConditionTestClass));
        }
    }
}