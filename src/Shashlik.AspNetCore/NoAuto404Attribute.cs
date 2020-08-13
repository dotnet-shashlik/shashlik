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
    /// 不要自动404
    /// </summary>    
    public class NoAuto404Attribute : ActionFilterAttribute
    {
    }
}
