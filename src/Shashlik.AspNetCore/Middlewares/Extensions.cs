using Microsoft.AspNetCore.Builder;

namespace Shashlik.AspNetCore.Middlewares
{
    public static class Extensions
    {
        /// <summary>
        /// 启用Request.Body 重复读
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseEnableBuffering(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EnableBufferingMiddleware>();
        }
    }
}