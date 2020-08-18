using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Shashlik.Kernel.Autowire
{
    /// <summary>
    /// 自动装配初始化器
    /// </summary>
    public interface IAutowireProvider
    {
        /// <summary>
        /// 执行装配
        /// </summary>
        /// <param name="pipelineService"></param>
        /// <param name="autowireAction"></param>
        void Autowire(IDictionary<TypeInfo, AutowireDescriptor> pipelineService, Action<AutowireDescriptor> autowireAction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="services"></param>
        /// <param name="replaces"></param>
        /// <param name="removes"></param>
        /// <param name="dependencyContext"></param>
        /// <returns></returns>
        IDictionary<TypeInfo, AutowireDescriptor> LoadFrom(
            TypeInfo baseType,
            IServiceCollection services,
            IEnumerable<TypeInfo> removes = null,
            DependencyContext dependencyContext = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="replaces"></param>
        /// <param name="removes"></param>
        /// <param name="dependencyContext"></param>
        /// <returns></returns>
        IDictionary<TypeInfo, AutowireDescriptor> LoadFrom(
            TypeInfo baseType,
            IServiceProvider serviceProvider,
            IEnumerable<TypeInfo> removes = null,
            DependencyContext dependencyContext = null);

        /// <summary>
        /// 扫描特性类型
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="dependencyContext"></param>
        /// <returns></returns>
        IDictionary<TypeInfo, AutowireDescriptor> LoadFromAttribute(TypeInfo attiributeType, DependencyContext dependencyContext = null, bool inherit = true);
    }
}
