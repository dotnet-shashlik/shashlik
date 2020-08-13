using Guc.Kernel.Exception;
using Guc.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Guc.AspNetCore
{
    /// <summary>
    /// 异常处理
    /// </summary>
    public class ExceptionFilter : IExceptionFilter
    {
        public ExceptionFilter(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
        }

        ILoggerFactory LoggerFactory { get; }

        public void OnException(ExceptionContext context)
        {
            if (context.Filters.Any(r => r is NoGucAspNetCoreAttribute))
                // 无需处理
                return;

            if (context.Exception is AggregateException)
                context.Exception = (context.Exception as AggregateException)?.InnerException;
            var logger = LoggerFactory.CreateLogger(context.Exception.Source);

            if (context.Exception is GucException ex)
            {
                if (ex.Code == Guc.Kernel.Exception.ExceptionCodes.Instance.NotFound)
                    context.Result = new NotFoundResult();
                else if (ex.Code == Guc.Kernel.Exception.ExceptionCodes.Instance.Forbid)
                    context.Result = new StatusCodeResult(Guc.Kernel.Exception.ExceptionCodes.Instance.Forbid);
                else if (ex.Code == Guc.Kernel.Exception.ExceptionCodes.Instance.UnAuth)
                    context.Result = new StatusCodeResult(Guc.Kernel.Exception.ExceptionCodes.Instance.UnAuth);
                else if (!context.Filters.Any(r => r is NoWrapperAttribute))
                    context.Result = new JsonResult(new ResponseResult(context.Exception.Message, ex.Code) { Debug = ex.Debug });

                if (!ex.LogContent.IsNullOrWhiteSpace())
                    logger.LogError($"[{ex.Code}]{ex.LogContent}");
            }

            if (context.Exception is DbUpdateConcurrencyException concurrencyException)
            {
                context.Result = new JsonResult(new ResponseResult("系统繁忙，请稍后重试", 500) { Debug = concurrencyException.Message });
            }

            var exList = context.HttpContext.RequestServices.GetServices<IOnException>();
            exList?.Foreach(r => r.OnException(context.Exception));
        }
    }
}
