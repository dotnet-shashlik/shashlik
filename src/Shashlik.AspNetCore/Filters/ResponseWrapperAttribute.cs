using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Response;
using Shashlik.Utils.Extensions;

// ReSharper disable InvertIf

namespace Shashlik.AspNetCore.Filters
{
    /// <summary>
    /// 自动包装结果为<see cref="ResponseResult"/>
    /// </summary>
    public class ResponseWrapperAttribute : ActionFilterAttribute
    {
        private static AspNetCoreOptions Options { get; set; }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            Options ??= context.HttpContext.RequestServices.GetRequiredService<IOptions<AspNetCoreOptions>>().Value;

            if (context.Filters.Any(r => r is ResponseWrapperAttribute))
                // 无需处理
                return;

            switch (context.Result)
            {
                case EmptyResult _:
                    context.Result = new ObjectResult(new ResponseResult(Options.ResponseCode.Success, true,
                        Options.ResponseCode.SuccessDefaultMessage, null, null));
                    break;
                case ObjectResult result:
                {
                    if (result.DeclaredType != typeof(ResponseResult))
                        result.Value = new ResponseResult(Options.ResponseCode.Success, true,
                            Options.ResponseCode.SuccessDefaultMessage, result.Value, null);
                    break;
                }
                case ContentResult contentResult:
                    context.Result = new ObjectResult(new ResponseResult(Options.ResponseCode.Success, true,
                        Options.ResponseCode.SuccessDefaultMessage, contentResult.Content, null));
                    break;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Options ??= context.HttpContext.RequestServices.GetRequiredService<IOptions<AspNetCoreOptions>>().Value;

            if (!context.ModelState.IsValid && !context.Filters.Any(r => r is ResponseWrapperAttribute))
            {
                context.HttpContext.Response.StatusCode = Options.UseResponseExceptionToHttpCode ? 400 : 200;

                string error;
                if (!Options.ResponseAllModelError)
                    error = context.ModelState
                        .SelectMany(r => r.Value.Errors)
                        .FirstOrDefault(r => !r.ErrorMessage.IsNullOrWhiteSpace())
                        ?.ErrorMessage;
                else
                {
                    error = context.ModelState
                        .SelectMany(r => r.Value.Errors)
                        .Select(r => r.ErrorMessage)
                        .Join("\n");
                }

                context.Result = new ObjectResult(new ResponseResult(Options.ResponseCode.ArgError, false,
                    error ?? Options.ResponseCode.ArgErrorDefaultMessage, null, null));
            }
        }
    }
}