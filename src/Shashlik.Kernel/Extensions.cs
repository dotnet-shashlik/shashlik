using Guc.Kernel.Dependency;
using Guc.Kernel.Exception;
using Guc.Utils.Common;
using Guc.Utils.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Guc.Kernel
{
    /// <summary>
    /// 内核扩展类
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 增加guc kernel 公共服务,会自动注册应用中所有的约定服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dependencyContext">依赖上下文,null使用默认配置</param>

        /// <returns></returns>
        public static IKernelBuilder AddGucKernel(this IServiceCollection services, DependencyContext dependencyContext)
        {
            if (dependencyContext == null)
                throw new ArgumentNullException(nameof(dependencyContext));
            // 查找所有包含Guc.Kernel引用的程序集,并按约定进行服务注册
            var conventionAssemblies = AssemblyHelper.GetReferredAssemblies<IKernelBuilder>(dependencyContext);
            return services.AddGucKernel(conventionAssemblies);
        }

        /// <summary>
        /// 增加guc kernel 公共服务,会自动注册应用中所有的约定服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dependencyContext">依赖上下文,null使用默认配置</param>

        /// <returns></returns>
        public static IKernelBuilder AddGucKernel(this IServiceCollection services)
        {
            // 查找所有包含Guc.Kernel引用的程序集,并按约定进行服务注册
            var conventionAssemblies = AssemblyHelper.GetReferredAssemblies<IKernelBuilder>();
            return services.AddGucKernel(conventionAssemblies);
        }

        /// <summary>
        /// 增加guc kernel 公共服务,会自动注册应用中所有的约定服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dependencyContext">依赖上下文,null使用默认配置</param>

        /// <returns></returns>
        public static IKernelBuilder AddGucKernel(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            foreach (var item in assemblies)
                services.AddServiceByConvention(item);

            return
                new KernelBuilder(services)
                // 设置默认的异常状态码
                .SetExceptionCodes();
        }

        /// <summary>
        /// 设置异常状态码,不设置则为默认
        /// </summary>
        /// <param name="codesConfig">GucCodes配置</param>
        /// <returns></returns>
        public static IKernelBuilder SetExceptionCodes(this IKernelBuilder kernelBuilder, Func<ExceptionCodes> codesConfig = null)
        {
            var responseCodes = codesConfig?.Invoke();
            responseCodes = responseCodes ?? new ExceptionCodes();
            ExceptionCodes.Instance = responseCodes;
            return kernelBuilder;
        }

        /// <summary>
        /// guc kernel 配置
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IKernelConfig UseGucKernel(this IServiceProvider serviceProvider)
        {
            KernelServiceProvider.InitServiceProvider(serviceProvider);
            return new KernelConfig(serviceProvider);
        }
    }
}
