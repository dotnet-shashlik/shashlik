using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
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
    [ConditionDependsOnMissing(typeof(ICaptcha))]
    [ConditionOnProperty(typeof(bool?), "Shashlik.Captcha.Enable", true, null)]
    internal class DistributedCacheCatpcha : ICaptcha, ISingleton
    {
        public DistributedCacheCatpcha(IDistributedCache cache, IOptionsMonitor<CaptchaOptions> options)
        {
            Cache = cache;
            Options = options;
        }

        private IDistributedCache Cache { get; }
        private IOptionsMonitor<CaptchaOptions> Options { get; }

        /// <summary>
        /// 验证码是否正确
        /// </summary>
        /// <param name="purpose">验证类型,区分大小写</param>
        /// <param name="target">验证对象,target可以是邮件/手机等 都可以,区分大小写</param>
        /// <param name="code"></param>
        /// <param name="securityStamp">target当前的安全标识,比如用户修改了密码等验证码需要失效</param>
        /// <param name="isDeleteOnSucceed">验证成功后是否删除,totp无效</param>
        /// <returns></returns>
        public async Task<bool> IsValid(string purpose, string target, string code, string securityStamp = null,
            bool isDeleteOnSucceed = true)
        {
            var key = GetKey(purpose, target, securityStamp);

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
        /// <param name="purpose">验证类型,区分大小写</param>
        /// <param name="target">验证对象,target可以是邮件/手机等 都可以,区分大小写</param>
        /// <param name="securityStamp">target当前的安全标识,比如用户修改了密码等验证码需要失效</param>
        /// <param name="codeLength">验证码长度,totp无效</param>
        /// <returns></returns>
        public async Task<CodeModel> Build(string purpose, string target, string securityStamp = null,
            int codeLength = 6)
        {
            if (purpose == null) throw new ArgumentNullException(nameof(purpose));
            if (target == null) throw new ArgumentNullException(nameof(target));

            var key = GetKey(purpose, target, securityStamp);
            var now = DateTime.Now;

            var codeModel = new CodeModel
            {
                Code = RandomHelper.GetRandomCode(codeLength),
                Purpose = purpose,
                Target = target,
                ExpiresAt = now.AddSeconds(Options.CurrentValue.LifeTimeSecond),
                SendTime = now,
                ErrorCount = 0
            };

            await Cache.SetObjectAsync(key, codeModel, codeModel.ExpiresAt);
            return codeModel;
        }

        private string GetKey(string purpose, string target, string securityStamp = null)
        {
            if (securityStamp.IsNullOrWhiteSpace())
                return "CAPTCHA:{0}:{1}".Format(purpose, target);
            return "CAPTCHA:{0}:{1}:{2}".Format(purpose, target, securityStamp);
        }
    }
}