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

        /// <summary>
        /// 查找<see cref="ResponseException"/>异常深度
        /// </summary>
        public int FindDepthResponseException { get; set; } = 10;

        public override void OnException(ExceptionContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
            {
                if (actionDescriptor.MethodInfo.IsDefinedAttribute<NoExceptionWrapperAttribute>(true)
                    || actionDescriptor.MethodInfo.DeclaringType!.IsDefinedAttribute<NoExceptionWrapperAttribute>(true))
                    return;

                base.OnException(context);
                var options = context.HttpContext.RequestServices
                    .GetRequiredService<IOptions<AspNetCoreOptions>>().Value;

                var exception = context.Exception;
                for (var i = 0; i < FindDepthResponseException; i++)
                {
                    // ResponseException包装处理
                    if (exception is ResponseException responseException)
                    {
                        var errorCode =
                            options.ResponseCode.FormatCode(responseException.ResponseStatus,
                                responseException.ErrorCode);

                        var message = options.ResponseCode.FormatMessage(responseException.ResponseStatus,
                            responseException.Message);
                        var debug = options.IsDebug ? responseException.Debug : null;

                        var responseResult = new ResponseResult(errorCode, false, message, null, debug);
                        if (UseResponseExceptionToHttpCode)
                            context.HttpContext.Response.StatusCode = (int) ToHttpCode(responseException.ResponseStatus);
                        else
                            context.HttpContext.Response.StatusCode = (int) HttpStatusCode.OK;

                        context.Result = new JsonResult(responseResult);
                        var logger = context.HttpContext.RequestServices.GetService<ILoggerFactory>()
                            .CreateLogger(actionDescriptor.ControllerTypeInfo);
                        logger.LogInformation(context.Exception, $"Request has wrapped ResponseResult, debug: {responseResult.Debug ?? "empty"}");
                        return;
                    }
                    else if (exception.InnerException != null)
                        exception = exception.InnerException;
                }

                // 没有ResponseException异常,包装500系统错误
                {
                    var logger = context.HttpContext.RequestServices.GetService<ILoggerFactory>().CreateLogger(actionDescriptor.ControllerTypeInfo);
                    logger.LogError(context.Exception,
                        $"Action occur exception on request [{context.HttpContext.Request.Method}] {context.HttpContext.Request.Path}");
                    var debug = options.IsDebug ? exception.ToString() : null;
                    var responseResult = new ResponseResult(options.ResponseCode.SystemError, false,
                        options.ResponseCode.SystemErrorDefaultMessage, null, debug);

                    context.HttpContext.Response.StatusCode = UseResponseExceptionToHttpCode
                        ? (int) HttpStatusCode.InternalServerError
                        : (int) HttpStatusCode.OK;
                    context.Result = new JsonResult(responseResult);
                }
            }
        }


        private static HttpStatusCode ToHttpCode(ResponseStatus status)
        {
            return status switch
            {
                ResponseStatus.UnAuthentication => HttpStatusCode.Unauthorized,
                ResponseStatus.Forbidden => HttpStatusCode.Forbidden,
                ResponseStatus.NotFound => HttpStatusCode.NotFound,
                ResponseStatus.SystemError => HttpStatusCode.InternalServerError,
                _ => HttpStatusCode.BadRequest,
            };
        }
    }
}