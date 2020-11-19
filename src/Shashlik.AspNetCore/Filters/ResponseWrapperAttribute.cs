using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ResponseWrapperAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 是否将模型验证错误转换位http 200 ,默认ture
        /// </summary>
        public bool ModelError2HttpOk { get; set; } = true;

        /// <summary>
        /// 是否输出所有的模型验证错误
        /// </summary>
        public bool ResponseAllModelError { get; set; } = false;

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
                {
                    if (actionDescriptor.MethodInfo.IsDefinedAttribute<NoResponseWrapperAttribute>(true)
                        || actionDescriptor.MethodInfo.DeclaringType!.IsDefinedAttribute<NoResponseWrapperAttribute>(true)
                    )
                    {
                        await base.OnResultExecutionAsync(context, next);
                        return;
                    }
                }


                var options = context.HttpContext.RequestServices
                    .GetRequiredService<IOptions<AspNetCoreOptions>>().Value;
                context.HttpContext.Response.StatusCode =
                    ModelError2HttpOk ? (int) HttpStatusCode.OK : (int) HttpStatusCode.BadRequest;

                string? error;
                if (!ResponseAllModelError)
                    error = context.ModelState
                        .SelectMany(r => r.Value.Errors)
                        .FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.ErrorMessage))
                        ?.ErrorMessage;
                else
                {
                    error = context.ModelState
                        .SelectMany(r => r.Value.Errors)
                        .Select(r => r.ErrorMessage)
                        .Join(Environment.NewLine);
                }

                context.Result = new ObjectResult(new ResponseResult(options.ResponseCode.ArgError, false,
                    error ?? options.ResponseCode.ArgErrorDefaultMessage, null, null));
            }

            await base.OnResultExecutionAsync(context, next);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
            {
                if (actionDescriptor.MethodInfo.IsDefinedAttribute<NoResponseWrapperAttribute>(true)
                    || actionDescriptor.MethodInfo.DeclaringType!.IsDefinedAttribute<NoResponseWrapperAttribute>(true)
                )
                {
                    base.OnActionExecuted(context);
                    return;
                }


                base.OnActionExecuted(context);
                var options = context.HttpContext.RequestServices
                    .GetRequiredService<IOptions<AspNetCoreOptions>>().Value;
                switch (context.Result)
                {
                    case EmptyResult _:
                        context.Result = new ObjectResult(new ResponseResult(options.ResponseCode.Success, true,
                            options.ResponseCode.SuccessDefaultMessage, null, null));
                        break;
                    case ObjectResult result:
                    {
                        if (result.DeclaredType != typeof(ResponseResult))
                            result.Value = new ResponseResult(options.ResponseCode.Success, true,
                                options.ResponseCode.SuccessDefaultMessage, result.Value, null);
                        break;
                    }
                    case ContentResult contentResult:
                        context.Result = new ObjectResult(new ResponseResult(options.ResponseCode.Success, true,
                            options.ResponseCode.SuccessDefaultMessage, contentResult.Content, null));
                        break;
                }
            }
        }
    }
}