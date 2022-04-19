namespace Shashlik.Kernel
{
    /// <summary>
    /// Shashlik服务提供类,全局静态类
    /// </summary>
    public static class GlobalKernelServiceProvider
    {
        /// <summary>
        /// 根service provider
        /// </summary>
        public static IKernelServiceProvider? KernelServiceProvider { get; private set; }

        internal static void InitServiceProvider(IKernelServiceProvider serviceProvider)
        {
            KernelServiceProvider = serviceProvider;
        }
    }
}