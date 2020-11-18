using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Kernel.Dependency
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public abstract class ServiceAttribute : Attribute
    {
        protected ServiceAttribute(ServiceLifetime serviceLifetime, Type[] ignoreServices)
        {
            ServiceLifetime = serviceLifetime;
            IgnoreServices = ignoreServices ?? throw new ArgumentNullException(nameof(ignoreServices));
        }

        public ServiceLifetime ServiceLifetime { get; }

        public Type[] IgnoreServices { get; }

        /// <summary>
        /// 是否注册整个继承链
        /// </summary>
        public bool RequireRegistryInheritedChain { get; set; }
    }
}