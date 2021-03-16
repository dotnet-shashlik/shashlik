using System;
using System.Threading.Tasks;
using AspNetCore.Totp;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Extensions;

namespace Shashlik.Captcha.Totp
{
    /// <summary>
    /// totp验证码
    /// </summary>
    [ConditionOnProperty(typeof(bool), "Shashlik.Captcha.Enable", true, DefaultValue = true)]
    [Singleton]
    public class TotpCatpcha : ICaptcha
    {
        public TotpCatpcha(IOptionsMonitor<CaptchaOptions> options)
        {
            Options = options;
            TotpGenerator = new TotpGenerator();
            TotpValidator = new TotpValidator(TotpGenerator);
        }

        private IOptionsMonitor<CaptchaOptions> Options { get; }
        private TotpGenerator TotpGenerator { get; }
        private TotpValidator TotpValidator { get; }

        public Task<string> Build(string purpose, string target, int lifeTimeSeconds, int maxErrorCount, string code, string? securityStamp = null)
        {
            if (purpose is null) throw new ArgumentNullException(nameof(purpose));
            if (target is null) throw new ArgumentNullException(nameof(target));
            if (Options.CurrentValue.LifeTimeSecond <= 0)
                throw new InvalidOperationException("Invalid options: Shashlik.Captcha.LifeTimeSecond");
            if (Options.CurrentValue.MaxErrorCount < 0 || Options.CurrentValue.MaxErrorCount > 99)
                throw new InvalidOperationException("Invalid options: Shashlik.Captcha.MaxErrorCount, should be 0~99");

            var key = GetKey(purpose, target, securityStamp);
            return Task.FromResult(TotpGenerator.Generate(key).ToString());
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
        public Task<bool> IsValid(string purpose, string target, string code, string? securityStamp = null,
            bool isDeleteOnSucceed = true)
        {
            var key = GetKey(purpose, target, securityStamp);

            return Task.FromResult(
                code.TryParse<int>(out var codeInt)
                && TotpValidator.Validate(key, codeInt, Options.CurrentValue.LifeTimeSecond));
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="purpose">验证类型,区分大小写</param>
        /// <param name="target">验证对象,target可以是邮件/手机等 都可以,区分大小写</param>
        /// <param name="securityStamp">target当前的安全标识,比如用户修改了密码等验证码需要失效</param>
        /// <param name="codeLength">验证码长度,totp无效</param>
        /// <returns></returns>
        public Task<string> Build(string purpose, string target, string? securityStamp = null, int codeLength = 6)
        {
            return Build(purpose, target, 0, 0, string.Empty, securityStamp);
        }

        private static string GetKey(string purpose, string target, string? securityStamp = null)
        {
            if (securityStamp!.IsNullOrWhiteSpace())
                return ($"{purpose}:{target}");
            return $"{purpose}:{target}:{securityStamp}";
        }
    }
}