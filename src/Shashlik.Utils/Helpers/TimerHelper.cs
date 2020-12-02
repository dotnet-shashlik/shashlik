using System;
using System.Threading;
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
            if (cancellationToken.Value.IsCancellationRequested)
                return;

            if (expire <= TimeSpan.Zero)
                throw new ArgumentException("invalid expire.", nameof(expire));

            var timer = new Timer {Interval = expire.TotalMilliseconds};
            timer.Elapsed += (sender, e) =>
            {
                try
                {
                    timer.Enabled = false;
                    timer.Stop();
                    timer.Close();
                    timer.Dispose();
                }
                catch
                {
                    // ignored
                }

                if (!cancellationToken.Value.IsCancellationRequested)
                    action();
            };
            timer.Start();
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
            SetTimeout(action, (runAt - DateTimeOffset.Now), cancellationToken);
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
            if (cancellationToken.Value.IsCancellationRequested)
                return;
            if (interval <= TimeSpan.Zero)
                throw new ArgumentException("invalid interval.", nameof(interval));

            var timer = new Timer {Interval = interval.TotalMilliseconds};
            timer.Elapsed += (sender, e) =>
            {
                if (!cancellationToken.Value.IsCancellationRequested)
                    action();
                else
                    try
                    {
                        timer.Enabled = false;
                        timer.Stop();
                        timer.Close();
                        timer.Dispose();
                    }
                    catch
                    {
                        // ignored
                    }
            };
            timer.Start();
        }
    }
}