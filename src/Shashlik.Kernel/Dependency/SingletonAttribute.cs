using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Kernel.Dependency
{
    /// <summary>
    /// singleton inject, don't used on abstraction class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SingletonAttribute : ServiceAttribute
    {
        public SingletonAttribute() : base(ServiceLifetime.Singleton)
        {
        }
    }
}