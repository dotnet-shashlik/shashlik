using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Kernel.Dependency
{
    /// <summary>
    /// scoped inject, don't used on abstraction class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ScopedAttribute : ServiceAttribute
    {
        /// <summary>
        /// scoped service definition
        /// </summary>
        /// <param name="additionServiceType">will ignore service types</param>
        public ScopedAttribute(params Type[] additionServiceType) : base(ServiceLifetime.Scoped, additionServiceType, new Type[0])
        {
        }

        /// <summary>
        /// scoped service definition
        /// </summary>
        /// <param name="ignoreServiceType">will ignore service types</param>
        /// <param name="additionServiceType">will ignore service types</param>
        public ScopedAttribute(Type[] ignoreServiceType, params Type[] additionServiceType) : base(ServiceLifetime.Scoped, additionServiceType,
            ignoreServiceType)
        {
        }
    }
}