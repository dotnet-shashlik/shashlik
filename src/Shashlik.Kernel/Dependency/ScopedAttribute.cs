using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Kernel.Dependency
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ScopedAttribute : ServiceAttribute
    {
        /// <summary>
        /// scoped service definition
        /// </summary>
        /// <param name="ignoreServices">will ignore service types</param>
        public ScopedAttribute(params Type[] ignoreServices) : base(ServiceLifetime.Scoped, ignoreServices)
        {
        }
    }
}