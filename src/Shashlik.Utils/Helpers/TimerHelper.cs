using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Shashlik.Utils.Common
{
    public class TimerHelper
    {
        /// <summary>
        /// 在指定时间过后执行指定的表达式
        /// </summary>
        /// <param name="action">要执行的表达式</param>
        /// <param name="expire">过期时间</param>
        /// <param name="cancellationToken">撤销</param>
        /// <return>返回timer对象</return>
        public static void SetTimeout(Action action, TimeSpan expire, CancellationToken? cancellationToken = null)
        {
            Task.Run(() =>
            {
                Task.Delay((int)expire.TotalMilliseconds)
                .ContinueWith(task => action())
                .ConfigureAwait(false);
            }, cancellationToken ?? CancellationToken.None);
        }

        /// <summary>
        /// 定时执行任务
        /// </summary>
        /// <param name="action">要执行的表达式</param>
        /// <param name="interval">间隔时间</param>
        /// <param name="cancellationToken">撤销</param>
        /// <return>返回timer对象</return>
        public static void SetInterval(Action action, TimeSpan interval, CancellationToken? cancellationToken = null)
        {
            Task.Run(() =>
            {
                Task.Delay((int)interval.TotalMilliseconds)
                .ContinueWith(task => SetInterval(action, interval, cancellationToken))
                .ContinueWith(task => action())
                .ConfigureAwait(false);
            }, cancellationToken ?? CancellationToken.None);
        }
    }
}
