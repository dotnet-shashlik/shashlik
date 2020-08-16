using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;
using Shashlik.Kernel;
using Shashlik.Utils.Extensions;

namespace Shashlik.BackgroundJob
{
    public static class PostgreHangfireExtensions
    {
        /// <summary>
        /// 增加背景任务(使用redis存储, 务必先初始化redis)
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="configuration"></param>
        public static IKernelBuilder AddBackgroudJobWithRedis(this IKernelBuilder kernelBuilder, CSRedis.CSRedisClient redisClient = null)
        {
            kernelBuilder.Services.AddHangfire(r =>
            {
                r.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseRedisStorage(redisClient ?? RedisHelper.Instance);
            })
            .AddHangfireServer();

            return kernelBuilder;
        }

        /// <summary>
        /// 使用背景任务服务端
        /// </summary>
        /// <param name="application"></param>
        /// <param name="enableDashboard">是否启用仪表盘</param>
        /// <param name="zone">时区设置,默认为中国北京+8时区</param>
        public static IKernelAspNetCoreConfig UseBackgroudJobServer(
            this IKernelAspNetCoreConfig kernelAspNetCoreConfig,
            bool enableDashboard = false,
            TimeZoneInfo zone = null)
        {
            kernelAspNetCoreConfig.App.UseHangfireServer();
            if (enableDashboard)
                kernelAspNetCoreConfig.App.UseHangfireDashboard();

            // 得到所有的循环任务
            var allRecurringJob = kernelAspNetCoreConfig.ServiceProvider.GetServices<IRecurringJob>();
            if (!allRecurringJob.IsNullOrEmpty())
                // 统一添加到循环任务执行机
                foreach (var item in allRecurringJob)
                    RecurringJob.AddOrUpdate(() => item.Execute(), item.CronExpression, zone ?? TimeZoneInfo.Local);

            return kernelAspNetCoreConfig;
        }
    }
}
