using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Kernel.Dependency
{
    /// <summary>
    /// singleton inject, don't used on abstraction class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [Scoped( new[]{typeof(int)},typeof(String))]
    public class SingletonAttribute : ServiceAttribute
    {
        /// <summary>
        /// scoped service definition
        /// </summary>
        /// <param name="additionServiceType">will ignore service types</param>
        public SingletonAttribute(params Type[] additionServiceType) : base(ServiceLifetime.Scoped, additionServiceType, new Type[0])
        {
        }

        /// <summary>
        /// scoped service definition
        /// </summary>
        /// <param name="ignoreServiceType">will ignore service types</param>
        /// <param name="additionServiceType">will ignore service types</param>
        public SingletonAttribute(Type[] ignoreServiceType, params Type[] additionServiceType) : base(ServiceLifetime.Scoped, additionServiceType,
            ignoreServiceType)
        {
        }
    }
}