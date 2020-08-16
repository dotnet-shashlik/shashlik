using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Collections.Concurrent;
using System.Threading;

namespace Shashlik.AspNetCore
{
    /// <summary>
    /// 不要自动包裹api返回
    /// </summary>    
    public class NoWrapperAttribute : ActionFilterAttribute
    {
    }
}
