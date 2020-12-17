using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Kernel.Dependency
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class TransientAttribute : ServiceAttribute
    {
        /// <summary>
        /// transient service definition
        /// </summary>
        /// <param name="ignoreServices">will ignore service types</param>
        public TransientAttribute(params Type[] ignoreServices) : base(ServiceLifetime.Transient, ignoreServices)
        {
        }
    }
}