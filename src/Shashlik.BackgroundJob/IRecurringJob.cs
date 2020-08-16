using Shashlik.Kernel.Dependency;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.BackgroundJob
{
    /// <summary>
    /// 循环任务,单例
    /// </summary>
    public interface IRecurringJob : ISingleton, IDisposable
    {
        /// <summary>
        /// cron表达式,注意设置的时区
        /// </summary>
        string CronExpression { get; }

        /// <summary>
        /// 唯一的任务id
        /// </summary>
        string JobId { get; }

        /// <summary>
        /// 循环执行的逻辑
        /// </summary>
        void Execute();
    }
}
