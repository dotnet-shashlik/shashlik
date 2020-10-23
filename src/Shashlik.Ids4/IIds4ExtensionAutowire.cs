using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;

namespace Shashlik.Ids4
{
    /// <summary>
    /// ids4自定义扩展配置
    /// </summary>
    public interface IIds4ExtensionAutowire : IAutowire
    {
        void ConfigureIds4(IIdentityServerBuilder builder);
    }
}