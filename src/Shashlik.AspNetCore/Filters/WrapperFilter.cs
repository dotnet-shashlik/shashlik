using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Hosting;
using Guc.Utils;
using Guc.Kernel;

namespace Guc.AspNetCore
{
    /// <summary>
    /// 结果处理
    /// </summary>
    public class WrapperFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Filters.Any(r => r is NoGucAspNetCoreAttribute))
                // 无需处理
                return;

            // 包含NoWrapper特性的不对结果进行处理
            if (!context.Filters.Any(r => r is NoWrapperAttribute))
            {
                if (context.Result is EmptyResult)
                {
                    context.Result = new ObjectResult(new ResponseResult());
                }
                else if (context.Result is ObjectResult)
                {
                    var result = context.Result as ObjectResult;
                    if (result.Value == null && !context.Filters.Any(r => r is NoAuto404Attribute))
                        // 空,统一处理为404
                        context.Result = new NotFoundResult();
                    else if (result.DeclaredType != typeof(ResponseResult))
                        result.Value = new ResponseResult(result.Value);
                }
                else if (context.Result is ContentResult)
                {
                    context.Result = new ObjectResult(new ResponseResult((context.Result as ContentResult).Content));
                }
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Filters.Any(r => r is NoGucAspNetCoreAttribute))
                // 无需处理
                return;

            if (!context.Filters.Any(r => r is NoWrapperAttribute) && !context.ModelState.IsValid)
            {
                // 模型验证不通过的 统一返回参数错误
                context.HttpContext.Response.StatusCode = 200;
                context.Result = new ObjectResult(new ResponseResult("参数错误", Guc.Kernel.Exception.ExceptionCodes.Instance.ArgError));
            }
        }
    }
}
