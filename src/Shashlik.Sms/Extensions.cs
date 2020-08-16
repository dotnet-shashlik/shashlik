using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Sms.DomainSms;
using Shashlik.Kernel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Shashlik.Utils.Common;
using Shashlik.Utils.Extensions;
using System;

namespace Shashlik.Sms
{
    /// <summary>
    /// 短信服务扩展类
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 增加短信服务,可自定义实现ISmsInvoker,以增加其他的短信服务接口
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="configuration">短信配置节点</param>
        /// <param name="useEmpty">是否使用空短信</param>
        public static IKernelBuilder AddSms(this IKernelBuilder kernelBuilder, IConfiguration configuration, bool useEmpty = false)
        {
            if (useEmpty)
                return kernelBuilder.AddEmptySms(configuration);

            var services = kernelBuilder.Services;
            services.Configure<SmsOptions>(configuration);
            services.AddTransient<ISms, DefaultSms>();
            services.AddTransient<ISmsInvoker, AliSms>();
            services.AddTransient<ISmsInvoker, TencentSms>();

            return kernelBuilder;
        }

        /// <summary>
        /// 增加空短信服务,一般用于测试/开发环境
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="configuration">短信配置节点</param>
        public static IKernelBuilder AddEmptySms(this IKernelBuilder kernelBuilder, IConfiguration configuration)
        {
            var services = kernelBuilder.Services;
            services.Configure<SmsOptions>(configuration);
            services.AddTransient<ISms, EmptySms>();
            return kernelBuilder;
        }

        internal static bool IsPhone(this string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;
            return Regex.IsMatch(phone, @"^[1][0-9]{10}$");
        }

        internal static async Task<T> GetObjectAsync<T>(this IDistributedCache cache, string key)
            where T : class
        {
            var content = await cache.GetStringAsync(key);
            if (content.IsNullOrWhiteSpace())
                return null;
            return JsonHelper.Deserialize<T>(content);
        }

        internal static async Task SetObjectAsync(this IDistributedCache cache, string key, object obj, int expireSeconds)
        {
            await cache.SetStringAsync(key, obj.ToJson(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireSeconds)
            });
        }
    }
}
