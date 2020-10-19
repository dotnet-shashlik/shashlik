using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shashlik.Captcha;
using Shashlik.Identity.Options;

// ReSharper disable ClassNeverInstantiated.Global

namespace Shashlik.Identity
{
    /// <summary>
    /// ICaptcha通用验证码token提供类,可用于手机/邮件等验证码场景
    /// </summary>
    public class CaptchaTokenProvider<TUser> : IUserTwoFactorTokenProvider<TUser>
        where TUser : class
    {
        public CaptchaTokenProvider(ICaptcha captcha, IOptions<ShashlikIdentityOptions> options)
        {
            Captcha = captcha ??
                      throw new ArgumentException(
                          $"{typeof(CaptchaTokenProvider<TUser>)} require service {typeof(ICaptcha)}");
            Options = options;
        }

        private ICaptcha Captcha { get; }
        private IOptions<ShashlikIdentityOptions> Options { get; }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            var securityStamp = await manager.GetSecurityStampAsync(user);
            if (securityStamp == null) throw new ArgumentNullException(nameof(securityStamp));
            var target = GetTarget(await manager.GetUserIdAsync(user));
            var code = await Captcha.Build(purpose, target, securityStamp, Options.Value.CaptchaLength);
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
        public async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager,
            TUser user)
        {
            var securityStamp = await manager.GetSecurityStampAsync(user);
            if (securityStamp == null) throw new ArgumentNullException(nameof(securityStamp));
            var target = GetTarget(await manager.GetUserIdAsync(user));
            return await Captcha.IsValid(purpose, target, token, securityStamp);
        }

        /// <summary>
        /// 能否作为两步验证,这里永远返回true,这里只负责创建以及验证,至于验证码使用什么传输手段,这里并不关心,所以永远支持两步验证
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
        {
            return Task.FromResult(true);
        }

        private static string GetTarget(string id)
        {
            return id.ToString();
        }
    }
}