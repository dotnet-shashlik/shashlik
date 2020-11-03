using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Shashlik.AspNetCore;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Ids4
{
    /// <summary>
    /// ids4 server启动
    /// </summary>
    [Order(800)]
    public class Ids4IdentityServerAutowire : IAspNetCoreAutowire
    {
        public Ids4IdentityServerAutowire(IOptions<Ids4Options> options)
        {
            Options = options.Value;
        }

        private Ids4Options Options { get; }

        public void Configure(IApplicationBuilder app, IKernelServiceProvider _)
        {
            if (!Options.Enable)
                return;
            app.UseIdentityServer();
        }
    }
}