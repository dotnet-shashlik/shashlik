using System;
using Shashlik.Kernel;

namespace Shashlik.Validation
{
    public static class Extensions
    {

        /// <summary>
        /// 模型验证
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <returns></returns>
        public static IKernelService AddValidation(this IKernelService kernelBuilder)
        {
            return kernelBuilder;
        }

        /// <summary>
        /// 使用模型验证,自定义验证时的上下文的服务提供类获取方式
        /// </summary>
        /// <param name="kernelConfig"></param>
        /// <param name="validationContextServiceProvider">验证时的上下文的服务提供类获取方式</param>
        /// <returns></returns>
        public static IKernelConfig UseValidation(this IKernelConfig kernelConfig, Func<IServiceProvider> validationContextServiceProvider)
        {
            if (validationContextServiceProvider == null)
            {
                throw new ArgumentNullException(nameof(validationContextServiceProvider));
            }

            ValidationExtensions.SetShashlikValidation(validationContextServiceProvider);

            return kernelConfig;
        }

        /// <summary>
        /// 使用模型验证
        /// </summary>
        /// <param name="kernelConfig">使用根服务</param>
        /// <returns></returns>
        public static IKernelConfig UseValidation(this IKernelConfig kernelConfig)
        {
            ValidationExtensions.SetShashlikValidation(kernelConfig.ServiceProvider);

            return kernelConfig;
        }
    }
}
