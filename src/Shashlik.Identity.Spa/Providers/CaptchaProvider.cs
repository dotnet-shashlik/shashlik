using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shashlik.Captcha;
using Shashlik.Identity.Entities;
using Shashlik.Identity.Spa;

// ReSharper disable ClassNeverInstantiated.Global

namespace Shashlik.Identity.AspNetCore.Providers
{
    /// <summary>
    /// ICaptcha通用验证码token提供类,可用于手机/邮件等验证码场景
    /// </summary>
    public class CaptchaProvider : IUserTwoFactorTokenProvider<Users>
    {
        public CaptchaProvider(ICaptcha captcha, IOptions<ShashlikAspNetIdentityOptions> options)
        {
            Captcha = captcha;
            Options = options;
        }

        private ICaptcha Captcha { get; }
        private IOptions<ShashlikAspNetIdentityOptions> Options { get; }


        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> GenerateAsync(string purpose, UserManager<Users> manager, Users user)
        {
            var securityStamp = await manager.GetSecurityStampAsync(user);
            if (securityStamp == null) throw new ArgumentNullException(nameof(securityStamp));
            var target = GetTarget(await manager.GetUserIdAsync(user), securityStamp);
            var code = await Captcha.Build(purpose, target, Options.Value.CaptchaLength);
            return code.Code;
        }

        /// <summary>
        /// 验证验证码
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="token"></param>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> ValidateAsync(string purpose, string token, UserManager<Users> manager,
            Users user)
        {
            var securityStamp = await manager.GetSecurityStampAsync(user);
            if (securityStamp == null) throw new ArgumentNullException(nameof(securityStamp));
            var target = GetTarget(await manager.GetUserIdAsync(user), securityStamp);
            return await Captcha.IsValid(purpose, target, token);
        }

        /// <summary>
        /// 能否作为两步验证,这里永远返回true,这里值负责创建以及验证,至于验证码使用什么传输手段,这里并不关心,所以永远支持两步验证
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<Users> manager, Users user)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// 计算验证码目标,userid+securityStamp,即用户有更换securityStamp的操作,验证码将无效
        /// </summary>
        /// <param name="unifier"></param>
        /// <param name="securityStamp"></param>
        /// <returns></returns>
        private static string GetTarget(string unifier, string securityStamp)
        {
            return $"{unifier}:{securityStamp}";
        }
    }
}