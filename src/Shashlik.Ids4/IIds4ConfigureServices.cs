using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Ids4
{
    public interface IIds4ConfigureServices
    {
        void ConfigureIds4(IIdentityServerBuilder builder);
    }
}