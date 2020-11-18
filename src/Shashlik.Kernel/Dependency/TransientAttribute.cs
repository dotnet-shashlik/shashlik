using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Kernel.Dependency
{
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