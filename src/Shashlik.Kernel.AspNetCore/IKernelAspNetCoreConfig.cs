using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;

namespace Shashlik.Kernel
{
    /// <summary>
    /// Shashlik kernel 配置
    /// </summary>
    public interface IKernelAspNetCoreConfig : IKernelConfigure
    {
        IApplicationBuilder App { get; }
    }

    class KernelAspNetCoreConfig : IKernelAspNetCoreConfig
    {
        public KernelAspNetCoreConfig(IApplicationBuilder app)
        {
            ServiceProvider = app.ApplicationServices;
            App = app;
        }

        public IApplicationBuilder App { get; }
        public IServiceProvider ServiceProvider { get; }
    }
}
