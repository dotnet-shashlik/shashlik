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
        public TransientAttribute() : base(ServiceLifetime.Transient)
        {
        }
    }
}