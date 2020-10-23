using Microsoft.AspNetCore.Builder;
using Shashlik.Kernel;

// ReSharper disable MemberCanBePrivate.Global

namespace Shashlik.AspNetCore
{
    /// <summary>
    /// 内核扩展类
    /// </summary>
    public static class AspNetCoreExtensions
    {
        /// <summary>
        /// 自动配置IAutoAspNetConfigure
        /// </summary>
        /// <param name="kernelServiceProvider"></param>
        /// <param name="app"></param>
        public static IKernelServiceProvider AutowireAspNet(this IKernelServiceProvider kernelServiceProvider,
            IApplicationBuilder app)
        {
            return kernelServiceProvider.Autowire<IAspNetCoreAutowire>(r => r.Configure(app, kernelServiceProvider));
        }
    }
}