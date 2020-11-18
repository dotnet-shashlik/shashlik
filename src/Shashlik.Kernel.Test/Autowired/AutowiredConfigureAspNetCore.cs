using Microsoft.AspNetCore.Builder;
using Shashlik.AspNetCore;

namespace Shashlik.Kernel.Test.Autowired
{
    public class AutowiredConfigureAspNetCore : IAspNetCoreAutowire
    {
        public static bool Inited { get; private set; } = false;

        public void Configure(IApplicationBuilder app, IKernelServiceProvider _)
        {
            Inited = true;
        }
    }
}