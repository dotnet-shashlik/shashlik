﻿using Shashlik.Utils.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Shashlik.Kernel.Dependency.Conditions;
using System.Text.Json;
using Shashlik.Utils.Helpers;

namespace Shashlik.Captcha
{
    /// <summary>
    /// 验证码
    /// </summary>
    [DependsOn(typeof(IDistributedCache))]
    [ConditionOnProperty("Shashlik:Captcha.Enable", "true")]
    class DistributedCacheCatpcha : ICaptcha, Shashlik.Kernel.Dependency.ISingleton
    {
        public DistributedCacheCatpcha(IDistributedCache cache, IOptionsMonitor<CaptchaOptions> options)
        {
            this.Cache = cache;
            this.Options = options;
        }

        private IDistributedCache Cache { get; }

        private IOptionsMonitor<CaptchaOptions> Options { get; }

        // 0:subject,1:target
        private const string CachePrefix = "CAPTCHA:{0}:{1}";

        /// <summary>
        /// 验证code是否正确
        /// </summary>
        /// <param name="subject">验证码类型</param>
        /// <param name="target">验证目标</param>
        /// <param name="code">验证码</param>
        /// <param name="isDeleteOnSucceed"></param>
        /// <returns></returns>
        public async Task<bool> IsValid(string subject, string target, string code, bool isDeleteOnSucceed = true)
        {
            var key = CachePrefix.Format(subject, target);

            var codeModel = await Cache.GetObjectAsync<CodeModel>(key);
            if (codeModel == null)
                return false;

            if (code == codeModel.Code)
            {
                if (isDeleteOnSucceed)
                    await Cache.RemoveAsync(key);
                return true;
            }

            codeModel.ErrorCount += 1;
            if (codeModel.ErrorCount >= Options.CurrentValue.MaxErrorCount)
            {
                await Cache.RemoveAsync(key);
                return false;
            }

            var expire = codeModel.ExpiresAt - DateTime.Now.GetLongDate();
            if (expire <= 0)
                await Cache.RemoveAsync(key);
            else
                await Cache.SetObjectAsync(key, codeModel, (int) expire);
            return false;
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="subject">验证类型</param>
        /// <param name="target">验证目标</param>
        /// <param name="codeLength"></param>
        /// <returns></returns>
        public async Task<CodeModel> Build(string subject, string target, int codeLength = 6)
        {
            if (subject == null) throw new ArgumentNullException(nameof(subject));
            if (target == null) throw new ArgumentNullException(nameof(target));

            var key = CachePrefix.Format(subject, target);
            var now = DateTime.Now.GetLongDate();

            var verifyCode = new CodeModel
            {
                Code = RandomHelper.GetRandomCode(codeLength),
                Subject = subject,
                Target = target,
                ExpiresAt = now + Options.CurrentValue.ExpireSecond,
                SendTime = now,
                ErrorCount = 0
            };

            await Cache.SetObjectAsync(key, verifyCode, Options.CurrentValue.ExpireSecond);
            return verifyCode;
        }
    }

    internal static class Extensions
    {
        internal static async Task<T> GetObjectAsync<T>(this IDistributedCache cache, string key)
            where T : class
        {
            var content = await cache.GetStringAsync(key);
            return content.IsNullOrWhiteSpace() ? null : JsonSerializer.Deserialize<T>(content);
        }

        internal static async Task SetObjectAsync(this IDistributedCache cache, string key, object obj,
            int expireSeconds)
        {
            await cache.SetStringAsync(key, JsonSerializer.Serialize(obj), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireSeconds)
            });
        }
    }
}