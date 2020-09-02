using Shashlik.Kernel.Autowired;

namespace Shashlik.Kernel.Test.Autowired
{
    public class AutowiredConfigure : IAutowiredConfigure
    {
        public static bool Inited { get; private set; } = false;

        public void Configure(IKernelConfigure kernelConfigure)
        {
            Inited = true;
        }
    }
}