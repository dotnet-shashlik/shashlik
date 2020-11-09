using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Response;
using Shashlik.Utils.Extensions;

namespace Shashlik.AspNetCore.Filters
{
    /// <summary>
    /// 自动异常拦截,并返回<see cref="ResponseResult"/>数据
    /// </summary>    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExceptionWrapperAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// 是否将<see cref="ResponseException"/>异常转换为对应的http状态码,默认false
        /// </summary>
        public bool UseResponseExceptionToHttpCode { get; set; } = false;

        public override void OnException(ExceptionContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
            {
                if (actionDescriptor.MethodInfo.IsDefinedAttribute<ExceptionWrapperAttribute>(true)
                    || actionDescriptor.MethodInfo.DeclaringType.IsDefinedAttribute<ExceptionWrapperAttribute>(true))
                    return;

                base.OnException(context);
                var options = context.HttpContext.RequestServices
                    .GetRequiredService<IOptions<AspNetCoreOptions>>().Value;

                var exception = context.Exception;
                if (exception is AggregateException aggregateException)
                    exception = aggregateException?.InnerException;
                var loggerFactory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger(context.Exception.Source);

                // ResponseException包装处理
                if (exception is ResponseException responseException)
                {
                    var errorCode =
                        options.ResponseCode.GetCode(responseException.ResponseStatus, responseException.ErrorCode);

                    var debug = options.IsDebug ? responseException.Debug : null;
                    var responseResult = new ResponseResult(errorCode, false, responseException.Message, null,
                        responseException.Debug);
                    var httpCode = 200;
                    if (UseResponseExceptionToHttpCode && responseException.ResponseStatus == ResponseStatus.Other)
                        httpCode = (int) ToHttpCode(responseException.ResponseStatus);
                    context.Result = new StatusCodeResult(httpCode);
                    context.Result = new JsonResult(responseResult);
                }
                else
                {
                    var debug = options.IsDebug ? exception!.ToString() : null;
                    var responseResult = new ResponseResult(options.ResponseCode.SystemError, false,
                        options.ResponseCode.SystemErrorDefaultMessage, null, debug);
                    var httpCode = 200;
                    if (UseResponseExceptionToHttpCode)
                        httpCode = 500;

                    context.Result = new StatusCodeResult(httpCode);
                    context.Result = new JsonResult(responseResult);
                }
            }
        }


        private static HttpStatusCode ToHttpCode(ResponseStatus status)
        {
            return status switch
            {
                ResponseStatus.ArgError => HttpStatusCode.BadRequest,
                ResponseStatus.LogicalError => HttpStatusCode.Conflict,
                ResponseStatus.Unauthorized => HttpStatusCode.Unauthorized,
                ResponseStatus.Forbidden => HttpStatusCode.Forbidden,
                ResponseStatus.NotFound => HttpStatusCode.NotFound,
                ResponseStatus.SystemError => HttpStatusCode.InternalServerError,
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }
    }
}