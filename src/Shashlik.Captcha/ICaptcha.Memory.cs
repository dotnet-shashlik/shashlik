using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.Captcha.Redis
{
    /// <summary>
    /// Redis验证码
    /// </summary>
    [ConditionDependsOn(typeof(IMemoryCache))]
    [ConditionDependsOnMissing(typeof(ICaptcha))]
    [ConditionOnProperty(typeof(bool), "Shashlik.Captcha." + nameof(CaptchaOptions.Enable), true, DefaultValue = true)]
    [Singleton]
    public class MemoryCatpcha : ICaptcha
    {
        private IMemoryCache MemoryCache { get; }

        public MemoryCatpcha(IMemoryCache memoryCache)
        {
            MemoryCache = memoryCache;
        }

        /// <summary>
        /// 自定义生成验证码数据,内部自动生成验证码
        /// </summary>
        /// <param name="purpose">验证类型,区分大小写</param>
        /// <param name="target">验证对象,target可以是邮件/手机等 都可以,区分大小写</param>
        /// <param name="lifeTimeSeconds">验证码有效时间，单位</param>
        /// <param name="maxErrorCount">最大错误次数</param>
        /// <param name="captchaLength">验证码长度</param>
        /// <param name="securityStamp">target当前的安全标识,比如用户修改了密码等安全标识验证码需要失效</param>
        public async Task<string> Build(string purpose, string target, int lifeTimeSeconds = 300, int maxErrorCount = 3, int captchaLength = 6,
            string? securityStamp = null)
        {
            var code = RandomHelper.GetRandomCode(captchaLength);
            await Build(purpose, target, lifeTimeSeconds, maxErrorCount, code, securityStamp);
            return code;
        }

        /// <summary>
        /// 自定义生成验证码数据,调用方生成验证码
        /// </summary>
        /// <param name="purpose">验证类型,区分大小写</param>
        /// <param name="target">验证对象,target可以是邮件/手机等 都可以,区分大小写</param>
        /// <param name="lifeTimeSeconds">验证码有效时间，单位秒</param>
        /// <param name="maxErrorCount">最大错误次数</param>
        /// <param name="captcha">自行生成验证码并传入</param>
        /// <param name="securityStamp">target当前的安全标识,比如用户修改了密码等安全标识验证码需要失效</param>
        /// <returns></returns>
        public async Task<string> Build(string purpose, string target, int lifeTimeSeconds, int maxErrorCount, string captcha,
            string? securityStamp = null)
        {
            if (purpose is null) throw new ArgumentNullException(nameof(purpose));
            if (target is null) throw new ArgumentNullException(nameof(target));
            if (lifeTimeSeconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(lifeTimeSeconds));
            if (maxErrorCount < 0 || maxErrorCount > 99)
                throw new ArgumentOutOfRangeException(nameof(maxErrorCount));
            if (string.IsNullOrWhiteSpace(captcha)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(captcha));

            var key = GetKey(purpose, target, securityStamp);
            var model = new CaptchaModel();
            model._errorCounter = maxErrorCount;
            model._captcha = captcha;
            MemoryCache.Set(key, model, TimeSpan.FromSeconds(lifeTimeSeconds));
            return await Task.FromResult(captcha);
        }

        /// <summary>
        /// 验证码是否正确
        /// </summary>
        /// <param name="purpose">验证类型,区分大小写</param>
        /// <param name="target">验证对象,target可以是邮件/手机等 都可以,区分大小写</param>
        /// <param name="captcha">验证码</param>
        /// <param name="securityStamp">target当前的安全标识,比如用户修改了密码等验证码需要失效</param>
        /// <param name="isDeleteOnSucceed">验证成功后是否删除</param>
        /// <returns></returns>
        public async Task<bool> IsValid(string purpose, string target, string captcha, string? securityStamp = null,
            bool isDeleteOnSucceed = true)
        {
            var key = GetKey(purpose, target, securityStamp);
            var model = MemoryCache.Get<CaptchaModel>(key);
            if (model is null)
                return await Task.FromResult(false);
            if (model._captcha == captcha)
            {
                if (isDeleteOnSucceed)
                    MemoryCache.Remove(key);
                return await Task.FromResult(true);
            }

            if (Interlocked.Decrement(ref model._errorCounter) <= 0)
                MemoryCache.Remove(key);

            return await Task.FromResult(false);
        }

        private static string GetKey(string purpose, string target, string? securityStamp)
        {
            if (string.IsNullOrWhiteSpace(securityStamp))
                return "CAPTCHA:{0}:{1}".Format(purpose, target);
            return "CAPTCHA:{0}:{1}:{2}".Format(purpose, target, securityStamp);
        }

        private class CaptchaModel
        {
            public volatile int _errorCounter;
            public volatile string? _captcha;
        }
    }
}