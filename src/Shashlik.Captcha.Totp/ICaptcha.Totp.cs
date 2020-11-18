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
    /// totp验证码,使用 DataProtection的当前密钥keyId作为secret混淆,需要依赖DataProtection
    /// </summary>
    [ConditionDependsOnMissing(typeof(ICaptcha))]
    [ConditionOnProperty(typeof(bool), "Shashlik.Captcha.Enable", true, DefaultValue = true)]
    [Singleton]
    internal class TotpCatpcha : ICaptcha
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

        /// <summary>
        /// 验证码是否正确
        /// </summary>
        /// <param name="purpose">验证类型,区分大小写</param>
        /// <param name="target">验证对象,target可以是邮件/手机等 都可以,区分大小写</param>
        /// <param name="code"></param>
        /// <param name="securityStamp">target当前的安全标识,比如用户修改了密码等验证码需要失效</param>
        /// <param name="isDeleteOnSucceed">验证成功后是否删除,totp无效</param>
        /// <returns></returns>
        public Task<bool> IsValid(string purpose, string target, string code, string securityStamp = null,
            bool isDeleteOnSucceed = true)
        {
            var key = BuildTokenString(purpose, target, securityStamp);

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
        public Task<CodeModel> Build(string purpose, string target, string securityStamp = null, int codeLength = 6)
        {
            var key = BuildTokenString(purpose, target, securityStamp);
            var code = TotpGenerator.Generate(key);
            return Task.FromResult(new CodeModel
            {
                Purpose = purpose,
                Target = target,
                Code = code.ToString("D6"),
                ErrorCount = 0,
                SendTime = DateTimeOffset.Now,
                ExpiresAt = DateTimeOffset.Now.AddSeconds(Options.CurrentValue.LifeTimeSecond)
            });
        }

        private string BuildTokenString(string purpose, string target, string securityStamp = null)
        {
            if (securityStamp.IsNullOrWhiteSpace())
                return ($"{purpose}:{target}");
            return $"{purpose}:{target}:{securityStamp}";
        }
    }
}