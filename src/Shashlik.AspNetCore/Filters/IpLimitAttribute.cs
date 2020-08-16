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
    /// api接口调用限制,基于ip限制
    /// </summary>
    public class IpLimitAttribute : ActionFilterAttribute
    {
        /// <summary>
        ///  接口调用 次数限制
        /// </summary>
        private readonly static ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentQueue<DateTime>>> apiRateLimitData
            = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentQueue<DateTime>>>();

        public int Limit { get; set; }
        public float Rate { get; set; }
        /// <summary>
        /// ip限制,24小时内调用的次数
        /// </summary>
        /// <param name="limit">限制次数</param>
        /// <param name="rate">频率限制,单位秒</param>
        public IpLimitAttribute(int limit = 0, float rate = 0)
        {
            this.Limit = limit;
            this.Rate = rate;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (Limit > 0 || Rate > 0)
            {
                var now = DateTime.Now;
                var lastDay = now.AddDays(-1);
                var actionId = context.ActionDescriptor.Id;
                var ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
                var ipCount = apiRateLimitData.GetOrAdd(actionId, new ConcurrentDictionary<string, ConcurrentQueue<DateTime>>());
                var times = ipCount.GetOrAdd(ip, new ConcurrentQueue<DateTime>());
                if (Limit > 0 && (times.Count(r => r > lastDay) >= Limit))
                {
                    context.Result = new JsonResult(new ResponseResult("操作太过频繁,请稍作歇息", 2));
                    Console.WriteLine($"Ip[{ip}]24小时请求[{context.ActionDescriptor.DisplayName}]超过限制次数:{Limit}次");
                    return;
                }
                if (Rate > 0 && times.Any() && times.Last().AddSeconds(Rate) >= now)
                {
                    context.Result = new JsonResult(new ResponseResult("操作太过频繁,请稍作歇息", 2));
                    Console.WriteLine($"ip[{ip}]频繁请求[{context.ActionDescriptor.DisplayName}]超过限制频率:{Rate}秒/次");
                    return;
                }

                Task.Run(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(1);
                        if (!times.Any() || times.First() > lastDay)
                        {
                            times.Enqueue(now);
                            return;
                        }

                        times.TryDequeue(out DateTime time);
                    }
                });
            }
            base.OnActionExecuting(context);
        }
    }
}
