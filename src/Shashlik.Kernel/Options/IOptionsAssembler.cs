namespace Shashlik.Kernel.Options
{
    public interface IOptionsAssembler
    {
        /// <summary>
        /// 配置options
        /// </summary>
        /// <param name="kernelServices"></param>
        void ConfigureAll(IKernelServices kernelServices);
    }
}