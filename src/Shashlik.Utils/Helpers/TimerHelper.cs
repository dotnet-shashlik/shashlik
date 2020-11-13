using System;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace Shashlik.Utils.Helpers
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
            cancellationToken ??= CancellationToken.None;

            if (expire <= TimeSpan.Zero)
                throw new ArgumentException("invalid expire.", nameof(expire));
            Task.Delay((int) expire.TotalMilliseconds, cancellationToken.Value)
                .ContinueWith(task => action(), cancellationToken.Value)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// 在指定时间执行指定的表达式
        /// </summary>
        /// <param name="action">要执行的表达式</param>
        /// <param name="runAt">过期时间</param>
        /// <param name="cancellationToken">撤销</param>
        /// <return>返回timer对象</return>
        public static void SetTimeout(Action action, DateTimeOffset runAt,
            CancellationToken? cancellationToken = null)
        {
            cancellationToken ??= CancellationToken.None;

            if (runAt <= DateTimeOffset.Now)
                throw new ArgumentException("invalid run time.", nameof(runAt));


            Task.Delay((int) (runAt - DateTimeOffset.Now).TotalMilliseconds, cancellationToken.Value)
                .ContinueWith(task => action(), cancellationToken.Value)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// 定时执行任务,不会立即执行
        /// </summary>
        /// <param name="action">要执行的表达式</param>
        /// <param name="interval">间隔时间</param>
        /// <param name="cancellationToken">撤销</param>
        /// <return>返回timer对象</return>
        public static void SetInterval(Action action, TimeSpan interval, CancellationToken? cancellationToken = null)
        {
            cancellationToken ??= CancellationToken.None;
            if (interval <= TimeSpan.Zero)
                throw new ArgumentException("invalid interval.", nameof(interval));
            Task.Delay((int) interval.TotalMilliseconds, cancellationToken.Value)
                .ContinueWith(task => SetInterval(action, interval, cancellationToken.Value))
                .ContinueWith(task => action(), cancellationToken.Value)
                .ConfigureAwait(false);
        }
    }
}