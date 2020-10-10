﻿using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Dependency;
using Shashlik.Kernel.Dependency.Conditions;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.Captcha.Totp
{
    /// <summary>
    /// totp验证码
    /// </summary>
    [DependsOn(typeof(IDistributedCache))]
    [ConditionOnProperty("Shashlik:Captcha.Enable", "true")]
    internal class TotpCatpcha : ICaptcha, ISingleton
    {
        public TotpCatpcha(IDataProtectionProvider dataProtectionProvider,
            IOptionsMonitor<CaptchaOptions> options)
        {
            DataProtectionProvider = dataProtectionProvider;
            Options = options;
            Rfc6238AuthenticationService.SetTimeStep(TimeSpan.FromSeconds(options.CurrentValue.LifeTimeSecond));
        }

        private IDataProtectionProvider DataProtectionProvider { get; }
        private IOptionsMonitor<CaptchaOptions> Options { get; }

        /// <summary>
        /// 验证code是否正确
        /// </summary>
        /// <param name="purpose">验证码类型</param>
        /// <param name="target">验证目标</param>
        /// <param name="code">验证码</param>
        /// <param name="isDeleteOnSucceed"></param>
        /// <returns></returns>
        public Task<bool> IsValid(string purpose, string target, string code, bool isDeleteOnSucceed = true)
        {
            return Task.FromResult(code.TryParse<int>(out var codeInt) &&
                                   Rfc6238AuthenticationService.ValidateCode(BuildToken(purpose, target), codeInt,
                                       null));
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="purpose">验证类型</param>
        /// <param name="target">验证目标</param>
        /// <param name="codeLength"></param>
        /// <returns></returns>
        public Task<CodeModel> Build(string purpose, string target, int codeLength = 6)
        {
            var code = Rfc6238AuthenticationService.GenerateCode(BuildToken(purpose, target), null);
            return Task.FromResult(new CodeModel
            {
                Id = code,
                Subject = purpose,
                Target = target,
                Code = code.ToString(),
                ErrorCount = 0,
                SendTime = DateTimeOffset.Now,
                ExpiresAt = DateTimeOffset.Now.AddSeconds(Options.CurrentValue.LifeTimeSecond)
            });
        }

        private byte[] BuildToken(string purpose, string target)
        {
            return Encoding.UTF8.GetBytes($"{purpose}:{target}");
        }
    }
}