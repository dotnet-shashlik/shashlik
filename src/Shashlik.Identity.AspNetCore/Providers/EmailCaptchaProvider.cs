using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shashlik.Captcha;
using Shashlik.Identity.Entities;

// ReSharper disable ClassNeverInstantiated.Global

namespace Shashlik.Identity.AspNetCore.Providers
{
    /// <summary>
    /// 邮件验证码token提供类,一般作为邮件验证码登录
    /// </summary>
    public class EmailCaptchaProvider : EmailTokenProvider<Users>
    {
        public EmailCaptchaProvider(ICaptcha captcha, IOptions<ShashlikAspNetIdentityOptions> options)
        {
            Captcha = captcha;
            Options = options;
        }

        private ICaptcha Captcha { get; }
        private IOptions<ShashlikAspNetIdentityOptions> Options { get; }


        public override async Task<string> GenerateAsync(string purpose, UserManager<Users> manager, Users user)
        {
            var securityStamp = await manager.GetSecurityStampAsync(user);
            var phone = await manager.GetEmailAsync(user);
            var target = GetTarget(phone, securityStamp);
            var code = await Captcha.Build(purpose, target, Options.Value.CaptchaLength);
            return code.Code;
        }

        public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<Users> manager,
            Users user)
        {
            var securityStamp = await manager.GetSecurityStampAsync(user);
            var phone = await manager.GetEmailAsync(user);
            var target = GetTarget(phone, securityStamp);
            return await Captcha.IsValid(purpose, target, token);
        }

        private static string GetTarget(string unifier, string securityStamp)
        {
            return $"{unifier}:{securityStamp}";
        }
    }
}