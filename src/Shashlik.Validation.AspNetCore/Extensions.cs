using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using Guc.Kernel;

namespace Guc.Validation
{
    public static class AspNetCoreExtensions
    {
        /// <summary>
        /// 增加aspnet core环境下的模型验证
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <returns></returns>
        public static IKernelBuilder AddValidationWithAspNetCore(this IKernelBuilder kernelBuilder)
        {
            kernelBuilder.AddValidation();
            kernelBuilder.Services.AddHttpContextAccessor();
            return kernelBuilder;
        }

        /// <summary>
        /// 使用模型验证,使用HttpContext上下文的服务
        /// </summary>
        /// <param name="kernelConfig"></param>
        /// <returns></returns>
        public static IKernelAspNetCoreConfig UseValidationWithAspNetCore(this IKernelAspNetCoreConfig kernelConfig)
        {
            ValidationExtensions.SetGucValidation(() => kernelConfig.ServiceProvider.GetService<IHttpContextAccessor>()?.HttpContext.RequestServices);
            return kernelConfig;
        }
    }
}
