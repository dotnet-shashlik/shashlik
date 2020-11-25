using IdentityServer4.Extensions;
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

            // V3->V4 migration: https://github.com/IdentityServer/IdentityServer4/issues/4592
            app.Use(async (context, next) =>
            {
                if (!string.IsNullOrWhiteSpace(Options.PublicOrigin))
                    context.SetIdentityServerOrigin(Options.PublicOrigin);
                if (!string.IsNullOrWhiteSpace(Options.BasePath))
                    context.SetIdentityServerBasePath(Options.BasePath);
                await next.Invoke();
            });

            app.UseIdentityServer();
        }
    }
}