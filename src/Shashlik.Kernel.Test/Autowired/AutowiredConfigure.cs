using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.Autowired
{
    [Transient]
    public class AutowiredConfigure : IServiceProviderAssembler
    {
        public static bool Inited { get; private set; } = false;

        public void Configure(IKernelServiceProvider kernelServiceProvider)
        {
            Inited = true;
        }
    }
}