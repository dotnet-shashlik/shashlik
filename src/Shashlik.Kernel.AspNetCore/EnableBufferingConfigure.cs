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
    public class EnableBufferingConfigure : IAutoAspNetConfigure
    {
        public void Configure(IApplicationBuilder App)
        {
            App.UseMiddleware<EnableBufferinMiddleware>();
        }

        public class EnableBufferinMiddleware
        {
            private readonly RequestDelegate _next;

            public EnableBufferinMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext context)
            {
                context.Request.EnableBuffering();
                await _next(context);
            }
        }
    }
}