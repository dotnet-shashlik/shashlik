using Guc.Utils.Extensions;
using Guc.Utils.Common;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Guc.Features.VerifyCode;

namespace Guc.Features
{
    /// <summary>
    /// 验证码
    /// </summary>
    class DistributedCacheVerifyCodeFeature : IVerifyCodeFeature
    {
        public DistributedCacheVerifyCodeFeature(IDistributedCache cache, IOptions<VerifyCodeOptions> options)
        {
            this.cache = cache;
            this.options = options;
        }

        IDistributedCache cache { get; }
        IOptions<VerifyCodeOptions> options { get; }
        const string cachePrefix = "VERIFY_CODE:";

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
            string key = cachePrefix + subject + "_" + target;

            var codeModel = await cache.GetObjectAsync<Codes>(key);
            if (codeModel == null)
                return false;

            if (code == codeModel.Code)
            {
                if (isDeleteOnSucceed)
                    await cache.RemoveAsync(key);
                return true;
            }

            codeModel.ErrorCount += 1;
            if (codeModel.ErrorCount >= options.Value.MaxErrorCount)
            {
                await cache.RemoveAsync(key);
                return false;
            }

            var expire = codeModel.ExpiresAt - DateTime.Now.GetLongDate();
            if (expire <= 0)
                cache.Remove(key);
            else
                await cache.SetObjectAsync(key, codeModel, (int)expire);
            return false;
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="subject">验证类型</param>
        /// <param name="target">验证目标</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        public async Task<Codes> Build(string subject, string target, int codeLength = 6)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("subject", nameof(subject));
            }

            if (string.IsNullOrWhiteSpace(target))
            {
                throw new ArgumentException("target", nameof(target));
            }

            if (subject.Length > 32)
                throw new ArgumentException("subject max length:32");
            if (target.Length > 512)
                throw new ArgumentException("target max length:512");
            if (codeLength > 32)
                throw new ArgumentException("code max length:32");

            string key = cachePrefix + subject + "_" + target;
            var now = DateTime.Now.GetLongDate();

            Codes verifyCode = new Codes
            {
                Code = RandomHelper.GetRandomCode(codeLength),
                Subject = subject,
                Target = target,
                ExpiresAt = now + options.Value.ExpireSecond,
                SendTime = now,
                ErrorCount = 0
            };

            await cache.SetObjectAsync(key, verifyCode, options.Value.ExpireSecond);
            return verifyCode;
        }
    }
}
