﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Dependency;
using Shashlik.Kernel.Dependency.Conditions;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.Captcha.DistributedCache
{
    /// <summary>
    /// 验证码
    /// </summary>
    [DependsOn(typeof(IDistributedCache))]
    [ConditionOnProperty("Shashlik:Captcha.Enable", "true")]
    internal class DistributedCacheCatpcha : ICaptcha, ISingleton
    {
        public DistributedCacheCatpcha(IDistributedCache cache, IOptionsMonitor<CaptchaOptions> options)
        {
            Cache = cache;
            Options = options;
        }

        private IDistributedCache Cache { get; }

        private IOptionsMonitor<CaptchaOptions> Options { get; }

        // 0:subject,1:target
        private const string CachePrefix = "CAPTCHA:{0}:{1}";

        /// <summary>
        /// 验证code是否正确
        /// </summary>
        /// <param name="purpose">验证码类型</param>
        /// <param name="target">验证目标</param>
        /// <param name="code">验证码</param>
        /// <param name="isDeleteOnSucceed"></param>
        /// <returns></returns>
        public async Task<bool> IsValid(string purpose, string target, string code, bool isDeleteOnSucceed = true)
        {
            var key = CachePrefix.Format(purpose, target);

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

            await Cache.SetObjectAsync(key, codeModel, codeModel.ExpiresAt);
            return false;
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="purpose">验证类型</param>
        /// <param name="target">验证目标</param>
        /// <param name="codeLength"></param>
        /// <returns></returns>
        public async Task<CodeModel> Build(string purpose, string target, int codeLength = 6)
        {
            if (purpose == null) throw new ArgumentNullException(nameof(purpose));
            if (target == null) throw new ArgumentNullException(nameof(target));

            var key = CachePrefix.Format(purpose, target);
            var now = DateTime.Now;

            var codeModel = new CodeModel
            {
                Code = RandomHelper.GetRandomCode(codeLength),
                Subject = purpose,
                Target = target,
                ExpiresAt = now.AddSeconds(Options.CurrentValue.LifeTimeSecond),
                SendTime = now,
                ErrorCount = 0
            };

            await Cache.SetObjectAsync(key, codeModel, codeModel.ExpiresAt);
            return codeModel;
        }
    }
}