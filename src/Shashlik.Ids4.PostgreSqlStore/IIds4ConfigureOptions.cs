using IdentityServer4.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Ids4
{
    public interface IIds4ConfigureOptions
    {
        void ConfigureIds4(IdentityServerOptions options);
    }
}