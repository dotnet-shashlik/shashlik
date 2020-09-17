using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shashlik.Captcha;
using Shashlik.Identity.Entities;

// ReSharper disable ClassNeverInstantiated.Global

namespace Shashlik.Identity.AspNetCore.Providers
{
    /// <summary>
    /// 手机验证码token提供类,替换默认hash算法提供类
    /// </summary>
    public class PhoneNumberCaptchaProvider : PhoneNumberTokenProvider<Users>
    {
        public PhoneNumberCaptchaProvider(ICaptcha captcha, IOptions<ShashlikAspNetIdentityOptions> options)
        {
            Captcha = captcha;
            Options = options;
        }

        private ICaptcha Captcha { get; }
        private IOptions<ShashlikAspNetIdentityOptions> Options { get; }


        public override async Task<string> GenerateAsync(string purpose, UserManager<Users> manager, Users user)
        {
            var securityStamp = await manager.GetSecurityStampAsync(user);
            var phone = await manager.GetPhoneNumberAsync(user);
            var target = GetTarget(phone, securityStamp);
            var code = await Captcha.Build(purpose, target, Options.Value.CaptchaLength);
            return code.Code;
        }

        public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<Users> manager,
            Users user)
        {
            var securityStamp = await manager.GetSecurityStampAsync(user);
            var phone = await manager.GetPhoneNumberAsync(user);
            var target = GetTarget(phone, securityStamp);
            return await Captcha.IsValid(purpose, target, token);
        }

        private static string GetTarget(string unifier, string securityStamp)
        {
            return $"{unifier}:{securityStamp}";
        }
    }
}