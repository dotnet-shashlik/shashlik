﻿using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel
{
    internal class InnerKernelService : IKernelServices
    {
        public InnerKernelService(
            IServiceCollection services,
            DependencyContext scanFromDependencyContext,
            IConfiguration rootConfiguration)
        {
            Services = services;
            ScanFromDependencyContext = scanFromDependencyContext;
            RootConfiguration = rootConfiguration;
        }

        public IServiceCollection Services { get; }

        public List<ShashlikServiceDescriptor> ShashlikServiceDescriptors { get; } =
            new List<ShashlikServiceDescriptor>();

        public DependencyContext ScanFromDependencyContext { get; }
        public IConfiguration RootConfiguration { get; }

        public void AddShashlikServiceDescriptors(IEnumerable<ShashlikServiceDescriptor> descriptors)
        {
            ShashlikServiceDescriptors.AddRange(descriptors);
        }
    }
}