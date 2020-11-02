using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Kernel.Test.Autowired.Condition
{
    [ConditionOnProperty(typeof(bool), "AbsoluteNotExists", true)]
    public class ConditionAutowire : IServiceAutowire
    {
        public void Configure(IKernelServices kernelServices)
        {
            kernelServices.Services.AddSingleton<ConditionAutowireShouldBeNull>();
        }
    }
}