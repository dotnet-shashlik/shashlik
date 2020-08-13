using Guc.Kernel.Dependency;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Guc.BackgroundJob
{
    /// <summary>
    /// 延迟任务
    /// </summary>
    public interface IScheduleJob
    {
        /// <summary>
        /// 推迟指定时间<paramref name="delay"/>执行
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delay"></param>
        void Execute(Expression<Action> action, TimeSpan delay);

        /// <summary>
        /// 在指定的时间<paramref name="excuteAt"/>执行
        /// </summary>
        /// <param name="action"></param>
        /// <param name="excuteAt"></param>
        void Execute(Expression<Action> action, DateTimeOffset excuteAt);
    }
}
