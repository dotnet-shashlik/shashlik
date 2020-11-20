using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;

namespace Shashlik.IdentityIds4.MySql.Tests
{
    public class IdentityId4TestAutowire: IServiceAutowire
    {
        public void Configure(IKernelServices kernelServices)
        {
            kernelServices.Services.AddAuthentication()
                .AddCookie()
                .AddIdentityServerAuthentication();
        }
    }
}