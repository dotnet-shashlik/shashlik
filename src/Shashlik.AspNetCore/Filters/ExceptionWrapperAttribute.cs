using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Response;

namespace Shashlik.AspNetCore.Filters
{
    /// <summary>
    /// 自动异常拦截,并返回<see cref="ResponseResult"/>数据
    /// </summary>    
    public class ExceptionWrapperAttribute : ExceptionFilterAttribute
    {
        private static AspNetCoreOptions Options { get; set; }

        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            Options ??= context.HttpContext.RequestServices.GetRequiredService<IOptions<AspNetCoreOptions>>().Value;

            var exception = context.Exception;
            if (exception is AggregateException aggregateException)
                exception = aggregateException?.InnerException;
            var loggerFactory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(context.Exception.Source);

            // ResponseException包装处理
            if (exception is ResponseException responseException)
            {
                var errorCode = responseException.ResponseStatus.HasValue
                    ? Options.ResponseCode.GetCode(responseException.ResponseStatus.Value)
                    : responseException.ErrorCode;

                var debug = Options.IsDebug ? responseException.Debug : null;
                var responseResult = new ResponseResult(errorCode, false, responseException.Message, null,
                    responseException.Debug);
                var httpCode = 200;
                if (Options.UseResponseExceptionToHttpCode && responseException.ResponseStatus.HasValue)
                    httpCode = (int) ToHttpCode(responseException.ResponseStatus.Value);
                context.Result = new StatusCodeResult(httpCode);
                context.Result = new JsonResult(responseResult);

                if (responseException.WriteLog)
                    logger.LogError(responseException, responseException.WriteLogMessage);
            }
            else
            {
                var debug = Options.IsDebug ? exception!.ToString() : null;
                var responseResult = new ResponseResult(Options.ResponseCode.SystemError, false,
                    Options.ResponseCode.SystemErrorDefaultMessage, null, debug);
                var httpCode = 200;
                if (Options.UseResponseExceptionToHttpCode)
                    httpCode = 500;

                context.Result = new StatusCodeResult(httpCode);
                context.Result = new JsonResult(responseResult);
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