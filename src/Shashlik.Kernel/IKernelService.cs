﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Kernel
{
    /// <summary>
    /// Shashlik 内核 服务配置
    /// </summary>
    public interface IKernelService
    {
        IServiceCollection Services { get; }
    }

    class KernelService : IKernelService
    {
        public KernelService(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
