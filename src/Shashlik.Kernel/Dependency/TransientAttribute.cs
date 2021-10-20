using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Kernel.Dependency
{
    /// <summary>
    /// transient inject, don't used on abstraction class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TransientAttribute : ServiceAttribute
    {
        /// <summary>
        /// scoped service definition
        /// </summary>
        /// <param name="additionServiceType">will ignore service types</param>
        public TransientAttribute(params Type[] additionServiceType) : base(ServiceLifetime.Scoped, additionServiceType, new Type[0])
        {
        }

        /// <summary>
        /// scoped service definition
        /// </summary>
        /// <param name="ignoreServiceType">will ignore service types</param>
        /// <param name="additionServiceType">will ignore service types</param>
        public TransientAttribute(Type[] ignoreServiceType, params Type[] additionServiceType) : base(ServiceLifetime.Scoped, additionServiceType,
            ignoreServiceType)
        {
        }
    }
}