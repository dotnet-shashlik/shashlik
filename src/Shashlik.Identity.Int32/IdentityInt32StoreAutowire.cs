using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable CheckNamespace

namespace Shashlik.Identity
{
    public class IdentityInt32StoreAutowire : IIdentityExtensionAutowire
    {
        public void Configure(IdentityBuilder identityBuilder)
        {
            identityBuilder.AddEntityFrameworkStores<ShashlikIdentityDbContext>();
        }
    }
}