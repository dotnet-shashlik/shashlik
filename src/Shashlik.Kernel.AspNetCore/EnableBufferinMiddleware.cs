using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Shashlik.Kernel
{
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