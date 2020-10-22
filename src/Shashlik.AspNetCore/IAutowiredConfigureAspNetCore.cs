using Microsoft.AspNetCore.Builder;
using Shashlik.Kernel;

namespace Shashlik.AspNetCore
{
    /// <summary>
    /// Shashlik aspnet core 配置
    /// </summary>
    public interface IAutowiredConfigureAspNetCore : Kernel.Dependency.ISingleton
    {
        void Configure(IApplicationBuilder app, IKernelConfigure kernelConfigure);
    }
}