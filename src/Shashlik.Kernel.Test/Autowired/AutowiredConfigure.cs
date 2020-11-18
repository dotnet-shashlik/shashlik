namespace Shashlik.Kernel.Test.Autowired
{
    public class AutowiredConfigure : IServiceProviderAutowire
    {
        public static bool Inited { get; private set; } = false;

        public void Configure(IKernelServiceProvider kernelServiceProvider)
        {
            Inited = true;
        }
    }
}