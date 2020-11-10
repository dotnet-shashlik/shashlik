using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Kernel.Test.Autowired.TestAutowireConditionClasses
{
    [ConditionOnProperty(typeof(bool), "absolute_not_exists", true, DefaultValue = false)]
    public class FailConditionAutowire : IServiceAutowire
    {
        public void Configure(IKernelServices kernelServices)
        {
            kernelServices.Services.AddSingleton(typeof(FailConditionTestClass));
        }
    }
}