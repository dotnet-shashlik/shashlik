﻿using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Kernel.Dependency
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class SingletonAttribute : ServiceAttribute
    {
        /// <summary>
        /// singleton service definition
        /// </summary>
        /// <param name="ignoreServices">will ignore service types</param>
        public SingletonAttribute(params Type[] ignoreServices) : base(ServiceLifetime.Singleton, ignoreServices)
        {
        }
    }
}