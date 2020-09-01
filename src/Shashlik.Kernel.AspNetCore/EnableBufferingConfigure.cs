using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Shashlik.Kernel;

namespace Shashlik.Kernel
{
    /// <summary>
    /// context.Request.EnableBuffering
    /// </summary>
    public class EnableBufferingConfigure : IAutowiredConfigureAspNetCore
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<EnableBufferinMiddleware>();
        }


    }
}