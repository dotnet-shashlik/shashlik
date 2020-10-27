using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable CheckNamespace

namespace Shashlik.Identity
{
    public class IdentityInt32StoreAutowire : IIdentityExtensionAutowire
    {
        public void Configure(IdentityBuilder identityBuilder)
        {
            identityBuilder.Services.TryAddScoped<ShashlikInt32UserManager>();
            identityBuilder.AddEntityFrameworkStores<ShashlikIdentityDbContext>();
        }
    }
}