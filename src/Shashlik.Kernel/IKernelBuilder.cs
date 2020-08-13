using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.Kernel
{
    /// <summary>
    /// guc 内核 服务配置
    /// </summary>
    public interface IKernelBuilder
    {
        IServiceCollection Services { get; }
    }

    class KernelBuilder : IKernelBuilder
    {
        public KernelBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
