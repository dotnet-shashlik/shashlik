﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.Captcha.DistributedCache
{
    /// <summary>
    /// 验证码
    /// </summary>
    [ConditionDependsOn(typeof(IDistributedCache))]
    [ConditionOnProperty(typeof(bool), "Shashlik.Captcha.Enable", true, DefaultValue = true)]
    [Singleton]
    public class DistributedCacheCatpcha : ICaptcha
    {
        public DistributedCacheCatpcha(IDistributedCache cache, IOptionsMonitor<CaptchaOptions> options, ILock @lock)
        {
            Cache = cache;
            Options = options;
            Locker = @lock;
        }

        private IDistributedCache Cache { get; }
        private IOptionsMonitor<CaptchaOptions> Options { get; }
        private ILock Locker { get; }

        public async Task<string> Build(string purpose, string target, int lifeTimeSeconds, int maxErrorCount, string code,
            string? securityStamp = null)
        {
            if (purpose is null) throw new ArgumentNullException(nameof(purpose));
            if (target is null) throw new ArgumentNullException(nameof(target));
            if (lifeTimeSeconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(lifeTimeSeconds));
            if (maxErrorCount < 0 || maxErrorCount > 99)
                throw new ArgumentOutOfRangeException(nameof(maxErrorCount));
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(code));

            var key = GetKey(purpose, target, securityStamp);
            var codeModel = new CaptchaModel
            {
                Code = code,
                ErrorCount = 0,
                MaxErrorCount = Options.CurrentValue.MaxErrorCount,
                ExpireAt = DateTimeOffset.Now.AddSeconds(lifeTimeSeconds)
            };
            await Cache.SetObjectWithJsonAsync(key, codeModel, DateTimeOffset.Now.AddSeconds(lifeTimeSeconds));
            return code;
        }

        /// <summary>
        /// 验证码是否正确
        /// </summary>
        /// <param name="purpose">验证类型,区分大小写</param>
        /// <param name="target">验证对象,target可以是邮件/手机等 都可以,区分大小写</param>
        /// <param name="code"></param>
        /// <param name="securityStamp">target当前的安全标识,比如用户修改了密码等验证码需要失效</param>
        /// <param name="isDeleteOnSucceed">验证成功后是否删除,totp无效</param>
        /// <returns></returns>
        public async Task<bool> IsValid(string purpose, string target, string code, string? securityStamp = null,
            bool isDeleteOnSucceed = true)
        {
            var key = GetKey(purpose, target, securityStamp);
            using var lck = Locker.Lock(key, 3);
            var codeModel = await Cache.GetObjectWithJsonAsync<CaptchaModel>(key);
            if (codeModel is null)
                return false;

            if (code == codeModel.Code)
            {
                if (isDeleteOnSucceed)
                    await Cache.RemoveAsync(key);
                return true;
            }

            codeModel.ErrorCount += 1;
            if (codeModel.ErrorCount >= codeModel.MaxErrorCount)
            {
                await Cache.RemoveAsync(key);
                return false;
            }

            await Cache.SetObjectWithJsonAsync(key, codeModel, codeModel.ExpireAt);
            return false;
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="purpose">验证类型,区分大小写</param>
        /// <param name="target">验证对象,target可以是邮件/手机等 都可以,区分大小写</param>
        /// <param name="securityStamp">target当前的安全标识,比如用户修改了密码等验证码需要失效</param>
        /// <param name="codeLength">验证码长度,totp无效</param>
        /// <returns></returns>
        public async Task<string> Build(string purpose, string target, string? securityStamp = null,
            int codeLength = 6)
        {
            var code = RandomHelper.GetRandomCode(codeLength);
            await Build(purpose, target, Options.CurrentValue.LifeTimeSecond, Options.CurrentValue.MaxErrorCount, code, securityStamp);
            return code;
        }

        private string GetKey(string purpose, string target, string? securityStamp = null)
        {
            if (string.IsNullOrWhiteSpace(securityStamp))
                return "CAPTCHA:{0}:{1}".Format(purpose, target);
            return "CAPTCHA:{0}:{1}:{2}".Format(purpose, target, securityStamp);
        }
    }
}