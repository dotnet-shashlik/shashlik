using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using NLog;
using Microsoft.Extensions.DependencyInjection;
using NLog.LayoutRenderers;
using Guc.Utils.Extensions;

namespace Guc.NLogger.Renders
{
    [LayoutRenderer("aspnet-request-response")]
    public class AspNetResponseRender : NLog.Web.LayoutRenderers.AspNetLayoutRendererBase
    {
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            if (HttpContextAccessor.HttpContext != null)
            {
                var body = HttpContextAccessor?.HttpContext?.RequestServices?.GetService<AspNetResponse>()?.Body;
                if (!body.IsNullOrWhiteSpace())
                    builder.AppendLine(body);
            }
        }
    }

    public class AspNetResponse : Guc.Kernel.Dependency.IScoped
    {
        public AspNetResponse(IHttpContextAccessor httpContextAccessor)
        {

            body = new Lazy<string>(() =>
            {
                try
                {
                    var context = httpContextAccessor.HttpContext;
                    if (context == null)
                        return null;

                    if (context.Response.ContentLength == 0)
                        return null;

                    if (context.Response.Body == null)
                        return null;

                    if (!context.Response.Body.CanRead)
                        return null;

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
