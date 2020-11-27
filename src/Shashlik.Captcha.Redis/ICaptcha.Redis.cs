using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CSRedis;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.Captcha.Redis
{
    /// <summary>
    /// 验证码
    /// </summary>
    [ConditionDependsOn(typeof(CSRedisClient))]
    [ConditionOnProperty(typeof(bool), "Shashlik.Captcha.Enable", true, DefaultValue = true)]
    [Singleton]
    public class RedisCacheCatpcha : ICaptcha
    {
        public RedisCacheCatpcha(CSRedisClient redisClient, IOptionsMonitor<CaptchaOptions> options)
        {
            RedisClient = redisClient;
            Options = options;
        }

        private CSRedisClient RedisClient { get; }
        private IOptionsMonitor<CaptchaOptions> Options { get; }

        public async Task Build(string purpose, string target, int lifeTimeSeconds, int maxErrorCount, string code, string securityStamp = null)
        {
            if (purpose is null) throw new ArgumentNullException(nameof(purpose));
            if (target is null) throw new ArgumentNullException(nameof(target));
            if (lifeTimeSeconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(lifeTimeSeconds));
            if (maxErrorCount < 0 || maxErrorCount > 99)
                throw new ArgumentOutOfRangeException(nameof(maxErrorCount));
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(code));

            var key = GetKey(purpose, target, securityStamp);
            var errorKey = $"{key}:ERROR";
            code = $"TOLERATE:{maxErrorCount:D2}:{code}";
            RedisClient.StartPipe()
                .Set(errorKey, 0, lifeTimeSeconds)
                .Set(key, code, lifeTimeSeconds)
                .EndPipe();
            await Task.CompletedTask;
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
        public async Task<bool> IsValid(string purpose, string target, string code, string securityStamp = null,
            bool isDeleteOnSucceed = true)
        {
            var key = GetKey(purpose, target, securityStamp);
            var errorKey = $"{key}:ERROR";
            var redisCode = RedisClient.Get<string>(key);
            if (redisCode is null)
                return false;

            int maxErrorCount = Options.CurrentValue.MaxErrorCount;
            var reg = new Regex("^TOLERATE:(\\d\\d):(.*)");
            var match = reg.Match(redisCode);
            if (match.Success && match.Groups.Count == 3)
            {
                redisCode = match.Groups[2].Value;
                maxErrorCount = match.Groups[1].Value.ParseTo<int>();
            }

            if (code == redisCode)
            {
                if (isDeleteOnSucceed)
                {
                    RedisClient.StartPipe()
                        .Del(key)
                        .Del(errorKey)
                        .EndPipe();
                }

                return true;
            }

            if (maxErrorCount == 0)
                return false;

            // 加1次错误数量
            var errorCount = await RedisClient.IncrByAsync(errorKey);
            if (errorCount >= maxErrorCount)
            {
                RedisClient.StartPipe()
                    .Del(key)
                    .Del(errorKey)
                    .EndPipe();
                return false;
            }

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
        public async Task<string> Build(string purpose, string target, string securityStamp = null,
            int codeLength = 6)
        {
            var code = RandomHelper.GetRandomCode(codeLength);
            await Build(purpose, target, Options.CurrentValue.LifeTimeSecond, Options.CurrentValue.MaxErrorCount, code, securityStamp);
            return code;
        }

        private static string GetKey(string purpose, string target, string securityStamp = null)
        {
            if (string.IsNullOrWhiteSpace(securityStamp))
                return "CAPTCHA:{0}:{1}".Format(purpose, target);
            return "CAPTCHA:{0}:{1}:{2}".Format(purpose, target, securityStamp);
        }
    }
}