using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Ids4
{
    /// <summary>
    /// ids4自定义扩展配置
    /// </summary>
    public interface IIdentityServerBuilderConfigure
    {
        void ConfigureIds4(IIdentityServerBuilder builder);
    }
}