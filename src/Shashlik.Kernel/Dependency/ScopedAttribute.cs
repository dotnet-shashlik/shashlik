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
        public ScopedAttribute() : base(ServiceLifetime.Scoped)
        {
        }
    }
}