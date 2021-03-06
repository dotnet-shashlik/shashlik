﻿using Microsoft.AspNetCore.Builder;
using Shashlik.Kernel;
using Shashlik.Kernel.Dependency;

namespace Shashlik.AspNetCore
{
    /// <summary>
    /// Shashlik aspnet core 配置
    /// </summary>
    public interface IAspNetCoreAutowire : IAutowire
    {
        void Configure(IApplicationBuilder app, IKernelServiceProvider kernelServiceProvider);
    }
}