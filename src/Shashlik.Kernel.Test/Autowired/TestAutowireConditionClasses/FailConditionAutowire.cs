using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.Autowired.TestAutowireConditionClasses
{
    [Transient]
    public class FailConditionAutowire : IServiceAssembler
    {
        public void Configure(IKernelServices kernelServices)
        {
            kernelServices.Services.AddSingleton(typeof(FailConditionTestClass));
        }
    }
}