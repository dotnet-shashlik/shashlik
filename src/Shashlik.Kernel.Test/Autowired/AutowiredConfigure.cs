using Shashlik.Kernel.Autowired;

namespace Shashlik.Kernel.Test.Autowired
{
    public class AutowiredConfigure : IServiceProviderAutowire
    {
        public static bool Inited { get; private set; } = false;

        public void Configure(IKernelServiceProvider kernelConfigure)
        {
            Inited = true;
        }
    }
}