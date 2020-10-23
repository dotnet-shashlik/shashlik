namespace Shashlik.Kernel.Autowire
{
    public interface IOptionsAutowire
    {
        IKernelServices ConfigureAll(IKernelServices kernelServices);
    }
}