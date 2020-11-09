using Microsoft.AspNetCore.Builder;
using Shashlik.AspNetCore.Middlewares;
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

        /// <summary>
        /// 启用Request.Body 重复读
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseEnableBuffering(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EnableBufferingMiddleware>();
        }
    }
}