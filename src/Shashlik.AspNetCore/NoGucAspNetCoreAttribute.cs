using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Collections.Concurrent;
using System.Threading;

namespace Guc.AspNetCore
{
    /// <summary>
    /// 无需处理 异常/结果等等
    /// </summary>    
    public class NoGucAspNetCoreAttribute : ActionFilterAttribute
    {
    }
}
