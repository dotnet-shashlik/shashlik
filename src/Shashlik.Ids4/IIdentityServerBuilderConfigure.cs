using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Ids4
{
    public interface IIdentityServerBuilderConfigure
    {
        void ConfigureIds4(IIdentityServerBuilder builder);
    }
}