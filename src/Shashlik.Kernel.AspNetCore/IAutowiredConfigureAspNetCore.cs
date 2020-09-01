using Microsoft.AspNetCore.Builder;

namespace Shashlik.Kernel
{
    /// <summary>
    /// Shashlik aspnet core 配置
    /// </summary>
    public interface IAutowiredConfigureAspNetCore : Shashlik.Kernel.Dependency.ISingleton
    {
        void Configure(IApplicationBuilder app);
    }
}
