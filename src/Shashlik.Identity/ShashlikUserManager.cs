using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Captcha;
using Shashlik.Identity.Entities;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Extensions;
using StringExtensions = RestSharp.Extensions.StringExtensions;

namespace Shashlik.Identity
{
    public class ShashlikUserManager : UserManager<Users>, IScoped
    {
        protected ShashlikIdentityDbContext DbContext { get; }

        public ShashlikUserManager(IUserStore<Users> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<Users> passwordHasher,
            IEnumerable<IUserValidator<Users>> userValidators,
            IEnumerable<IPasswordValidator<Users>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<Users>> logger, ShashlikIdentityDbContext identityDbContext)
            : base(store, optionsAccessor, passwordHasher, userValidators,
                passwordValidators, keyNormalizer, errors, services, logger)
        {
            DbContext = identityDbContext;
        }

        /// <summary>
        /// 根据身份证查找用户,手机号码
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<Users> FindByPhoneNumberAsync(string phoneNumber)
        {
            return await DbContext.Users.SingleOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        /// <summary>
        /// 根据身份证查找用户,身份证号码唯一有效
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        public async Task<Users> FindByIdCardAsync(string idCard)
        {
            idCard = idCard.ToUpperInvariant();
            return await DbContext.Users.SingleOrDefaultAsync(u => u.IdCard == idCard);
        }

        /// <summary>
        /// 获取totp算法两阶段登录的二维码内容
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<string> GetTotpTwoFactorQrcode(string userId)
        {
            // qrcode rules see-> https://github.com/google/google-authenticator/wiki/Key-Uri-Format
            const string authenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

            var user = await this.FindByIdAsync(userId);
            var key = await this.GetAuthenticatorKeyAsync(user);
            if (key.IsNullOrWhiteSpace())
                await this.ResetAuthenticatorKeyAsync(user);
            key = await this.GetAuthenticatorKeyAsync(user);

            return authenticatorUriFormat.Format("AspNetIdentity".UrlEncode(), StringExtensions.UrlEncode(userId), key);
        }

        /// <summary>
        /// 生成用于登录的验证码,使用Captcha
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<string> GenerateLoginCaptcha(string userId)
        {
            var user = await this.FindByIdAsync(userId);
            return await this.GenerateUserTokenAsync(user, ShashlikIdentityConsts.CaptchaTokenProvider,
                ShashlikIdentityConsts.LoginPurpose);
        }

        /// <summary>
        /// 生成用于登录的验证码,使用Captcha
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="catpcha"></param>
        /// <returns></returns>
        public async Task<bool> IsValidLoginCaptcha(string userId, string catpcha)
        {
            var user = await this.FindByIdAsync(userId);
            return await this.VerifyUserTokenAsync(user, ShashlikIdentityConsts.CaptchaTokenProvider,
                ShashlikIdentityConsts.LoginPurpose, catpcha);
        }

        /// <summary>
        /// 生成用于登录的验证码,使用Captcha
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> GenerateLoginCaptcha(Users user)
        {
            return await this.GenerateUserTokenAsync(user, ShashlikIdentityConsts.CaptchaTokenProvider,
                ShashlikIdentityConsts.LoginPurpose);
        }

        /// <summary>
        /// 生成用于登录的验证码,使用Captcha
        /// </summary>
        /// <param name="user"></param>
        /// <param name="catpcha"></param>
        /// <returns></returns>
        public async Task<bool> IsValidLoginCaptcha(Users user, string catpcha)
        {
            return await this.VerifyUserTokenAsync(user, ShashlikIdentityConsts.CaptchaTokenProvider,
                ShashlikIdentityConsts.LoginPurpose, catpcha);
        }
    }
}