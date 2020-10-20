using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
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
        /// <summary>
        /// 是否将<see cref="ResponseException"/>异常转换为对应的http状态码,默认false
        /// </summary>
        public bool UseResponseExceptionToHttpCode { get; set; } = false;

        /// <summary>
        /// 是否输入所有的模型验证错误
        /// </summary>
        public bool ResponseAllModelError { get; set; } = false;

        private static AspNetCoreOptions Options { get; set; }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
            {
                if (actionDescriptor.MethodInfo.IsDefinedAttribute<NoResponseWrapperAttribute>(true))
                    return;

                base.OnActionExecuted(context);
                Options ??= context.HttpContext.RequestServices.GetRequiredService<IOptions<AspNetCoreOptions>>().Value;

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
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Options ??= context.HttpContext.RequestServices.GetRequiredService<IOptions<AspNetCoreOptions>>().Value;

            if (!context.ModelState.IsValid)
            {
                context.HttpContext.Response.StatusCode = UseResponseExceptionToHttpCode ? 400 : 200;

                string error;
                if (!ResponseAllModelError)
                    error = context.ModelState
                        .SelectMany(r => r.Value.Errors)
                        .FirstOrDefault(r => !r.ErrorMessage.IsNullOrWhiteSpace())
                        ?.ErrorMessage;
                else
                {
                    error = context.ModelState
                        .SelectMany(r => r.Value.Errors)
                        .Select(r => r.ErrorMessage)
                        .Join(Environment.NewLine);
                }

                context.Result = new ObjectResult(new ResponseResult(Options.ResponseCode.ArgError, false,
                    error ?? Options.ResponseCode.ArgErrorDefaultMessage, null, null));
            }
        }
    }
}