using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Identity.Entities;
using Shashlik.Utils.Extensions;
using StringExtensions = RestSharp.Extensions.StringExtensions;

// ReSharper disable ClassNeverInstantiated.Global

namespace Shashlik.Identity
{
    public class ShashlikUserManager<TUsers, TKey> : UserManager<TUsers>
        where TKey : IEquatable<TKey>
        where TUsers : IdentityUserBase<TKey>
    {
        public ShashlikUserManager(IUserStore<TUsers> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<TUsers> passwordHasher,
            IEnumerable<IUserValidator<TUsers>> userValidators,
            IEnumerable<IPasswordValidator<TUsers>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<TUsers>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators,
                passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        /// <summary>
        /// 根据身份证查找用户,手机号码
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<TUsers> FindByPhoneNumberAsync(string phoneNumber)
        {
            return await Users.SingleOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        /// <summary>
        /// 根据身份证查找用户,身份证号码唯一有效
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        public async Task<TUsers> FindByIdCardAsync(string idCard)
        {
            idCard = idCard.ToUpperInvariant();
            return await Users.SingleOrDefaultAsync(u => u.IdCard == idCard);
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
        public async Task<string> GenerateLoginCaptcha(TUsers user)
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
        public async Task<bool> IsValidLoginCaptcha(TUsers user, string catpcha)
        {
            return await this.VerifyUserTokenAsync(user, ShashlikIdentityConsts.CaptchaTokenProvider,
                ShashlikIdentityConsts.LoginPurpose, catpcha);
        }
    }
}