using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using NLog;
using Microsoft.Extensions.DependencyInjection;
using NLog.LayoutRenderers;
using Shashlik.Utils.Extensions;

namespace Shashlik.NLogger.Renders
{
    [LayoutRenderer("aspnet-request-body")]
    public class AspNetBodyRender : NLog.Web.LayoutRenderers.AspNetLayoutRendererBase
    {
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            if (HttpContextAccessor.HttpContext != null)
            {
                var body = HttpContextAccessor?.HttpContext?.RequestServices?.GetService<AspNetBody>()?.Body;
                if (!body.IsNullOrWhiteSpace())
                    builder.AppendLine(body);
            }
        }
    }

    public class AspNetBody : Shashlik.Kernel.Dependency.IScoped
    {
        public AspNetBody(IHttpContextAccessor httpContextAccessor)
        {

            body = new Lazy<string>(() =>
            {
                try
                {
                    var context = httpContextAccessor.HttpContext;
                    if (context == null)
                        return null;

                    if (context.Request.ContentLength == 0)
                        return null;

                    if (!context.Request.Method.Equals("post", StringComparison.OrdinalIgnoreCase)
                            && !context.Request.Method.Equals("put", StringComparison.OrdinalIgnoreCase)
                            && !context.Request.Method.Equals("patch", StringComparison.OrdinalIgnoreCase))
                        return null;

                    if (context.Request.Body == null)
                        return null;

                    if (!context.Request.Body.CanRead)
                        return null;

                    // 最大10万个字符
                    var str = context.Request.Body.ReadToString().SubStringIfTooLong(100000);
                    return str;
                }
                catch
                {
                    return null;
                }
            });

        }

        private Lazy<string> body { get; }

        public string Body => body.Value;
    }
}
