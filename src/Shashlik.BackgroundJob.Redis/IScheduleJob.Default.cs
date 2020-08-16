using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Shashlik.BackgroundJob
{
    public class DefaultScheduleJob : IScheduleJob, Shashlik.Kernel.Dependency.ISingleton
    {
        public void Execute(Expression<Action> action, TimeSpan delay)
        {
            Hangfire.BackgroundJob.Schedule(action, delay);
        }

        public void Execute(Expression<Action> action, DateTimeOffset excuteAt)
        {
            Hangfire.BackgroundJob.Schedule(action, excuteAt);
        }
    }
}
