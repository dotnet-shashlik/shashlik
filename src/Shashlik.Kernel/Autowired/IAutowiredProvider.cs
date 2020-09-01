using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Shashlik.Kernel.Autowired
{
    /// <summary>
    /// 自动装配初始化器
    /// </summary>
    public interface IAutowiredProvider
    {
        /// <summary>
        /// 执行装配
        /// </summary>
        /// <param name="autowiredService"></param>
        /// <param name="autowiredAction"></param>
        void Autowired(IDictionary<TypeInfo, AutowiredDescriptor> autowiredService,
            Action<AutowiredDescriptor> autowiredAction);

        /// <summary>
        /// 从依赖上下文加载
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="services"></param>
        /// <param name="dependencyContext"></param>
        /// <returns></returns>
        IDictionary<TypeInfo, AutowiredDescriptor> LoadFrom(
            TypeInfo baseType,
            IServiceCollection services,
            DependencyContext dependencyContext = null);

        /// <summary>
        /// 从服务提供类加载
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        IDictionary<TypeInfo, AutowiredDescriptor> LoadFrom(
            TypeInfo baseType,
            IServiceProvider serviceProvider);

        /// <summary>
        /// 扫描特性类型
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="dependencyContext"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        IDictionary<TypeInfo, AutowiredDescriptor> LoadFrom(TypeInfo attributeType,
            DependencyContext dependencyContext = null, bool inherit = true);
    }
}