using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Shashlik.Kernel.Autowire
{
    /// <summary>
    /// 自动装配初始化器
    /// </summary>
    public interface IAutoInitializer
    {
        /// <summary>
        /// 执行初始化
        /// </summary>
        /// <param name="autoServices"></param>
        /// <param name="initAction"></param>
        void Init(IDictionary<TypeInfo, AutoDescriptor> autoServices, Action<AutoDescriptor> initAction);

        /// <summary>
        /// 扫描类型
        /// </summary>
        /// <typeparam name="TBaseType"></typeparam>
        /// <param name="replaces"></param>
        /// <param name="dependencyContext"></param>
        /// <returns></returns>
        IDictionary<TypeInfo, AutoDescriptor> LoadFrom(
            TypeInfo baseType,
            IDictionary<TypeInfo, TypeInfo> replaces = null,
            IEnumerable<TypeInfo> removes = null,
            DependencyContext dependencyContext = null);

        /// <summary>
        /// 扫描类型,从依赖上下文中扫描
        /// </summary>
        /// <typeparam name="TBaseType"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <param name="replaces"></param>
        /// <param name="dependencyContext"></param>
        /// <returns></returns>
        IDictionary<TypeInfo, AutoDescriptor> LoadFrom(
            TypeInfo baseType,
            IServiceProvider serviceProvider,
            IDictionary<TypeInfo, TypeInfo> replaces = null,
            IEnumerable<TypeInfo> removes = null,
            DependencyContext dependencyContext = null);

        /// <summary>
        /// 扫描特性类型
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="dependencyContext"></param>
        /// <returns></returns>
        IDictionary<TypeInfo, AutoDescriptor> LoadFromAttribute(TypeInfo attiributeType, DependencyContext dependencyContext = null, bool inherit = true);
    }
}
