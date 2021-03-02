using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Utils.Extensions;

// ReSharper disable ConditionIsAlwaysTrueOrFalse

// ReSharper disable ClassNeverInstantiated.Global

namespace Shashlik.Identity
{
    public class ShashlikUserManager<TUser, TKey> : UserManager<TUser>, IShashlikUserManager
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TKey>, IIdentityUser
    {
        public ShashlikUserManager(IUserStore<TUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<TUser> passwordHasher,
            IEnumerable<IUserValidator<TUser>> userValidators,
            IEnumerable<IPasswordValidator<TUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<TUser>> logger, IUserConfirmation<TUser> userConfirmation)
            : base(store, optionsAccessor, passwordHasher, userValidators,
                passwordValidators, keyNormalizer, errors, services, logger)
        {
            UserConfirmation = userConfirmation;
        }

        private IUserConfirmation<TUser> UserConfirmation { get; }

        private TUser AsUser(IIdentityUser user)
        {
            return (user as TUser)!;
        }

        public Task<IdentityResult> AccessFailedAsync(IIdentityUser user)
        {
            return base.AccessFailedAsync(AsUser(user));
        }

        public Task<IdentityResult> AddClaimAsync(IIdentityUser user, Claim claim)
        {
            return base.AddClaimAsync(AsUser(user), claim);
        }

        public Task<IdentityResult> AddClaimsAsync(IIdentityUser user, IEnumerable<Claim> claims)
        {
            return base.AddClaimsAsync(AsUser(user), claims);
        }

        public Task<IdentityResult> AddLoginAsync(IIdentityUser user, UserLoginInfo login)
        {
            return base.AddLoginAsync(AsUser(user), login);
        }

        public Task<IdentityResult> AddPasswordAsync(IIdentityUser user, string password)
        {
            return base.AddPasswordAsync(AsUser(user), password);
        }

        public Task<IdentityResult> AddToRoleAsync(IIdentityUser user, string role)
        {
            return base.AddToRoleAsync(AsUser(user), role);
        }

        public Task<IdentityResult> AddToRolesAsync(IIdentityUser user, IEnumerable<string> roles)
        {
            return base.AddToRolesAsync(AsUser(user), roles);
        }

        public Task<IdentityResult> ChangeEmailAsync(IIdentityUser user, string newEmail, string token)
        {
            return base.ChangeEmailAsync(AsUser(user), newEmail, token);
        }

        public Task<IdentityResult> ChangePasswordAsync(IIdentityUser user, string currentPassword, string newPassword)
        {
            return base.ChangePasswordAsync(AsUser(user), currentPassword, newPassword);
        }

        public Task<IdentityResult> ChangePhoneNumberAsync(IIdentityUser user, string phoneNumber, string token)
        {
            return base.ChangePhoneNumberAsync(AsUser(user), phoneNumber, token);
        }

        public Task<bool> CheckPasswordAsync(IIdentityUser user, string password)
        {
            return base.CheckPasswordAsync(AsUser(user), password);
        }

        public Task<IdentityResult> ConfirmEmailAsync(IIdentityUser user, string token)
        {
            return base.ConfirmEmailAsync(AsUser(user), token);
        }

        public Task<int> CountRecoveryCodesAsync(IIdentityUser user)
        {
            return base.CountRecoveryCodesAsync(AsUser(user));
        }

        public Task<IdentityResult> CreateAsync(IIdentityUser user)
        {
            return base.CreateAsync(AsUser(user));
        }

        public Task<IdentityResult> CreateAsync(IIdentityUser user, string password)
        {
            return base.CreateAsync(AsUser(user), password);
        }

        public Task<byte[]> CreateSecurityTokenAsync(IIdentityUser user)
        {
            return base.CreateSecurityTokenAsync(AsUser(user));
        }

        public Task<IdentityResult> DeleteAsync(IIdentityUser user)
        {
            return base.DeleteAsync(AsUser(user));
        }

        public async Task<IIdentityUser> FindIdentityUserByEmailAsync(string email)
        {
            return await base.FindByEmailAsync(email);
        }

        public async Task<IIdentityUser> FindIdentityUserByIdAsync(string userId)
        {
            return await base.FindByIdAsync(userId);
        }

        public async Task<IIdentityUser> FindIdentityUserByLoginAsync(string loginProvider, string providerKey)
        {
            return await base.FindByLoginAsync(loginProvider, providerKey);
        }

        public async Task<IIdentityUser> FindIdentityUserByNameAsync(string userName)
        {
            return await base.FindByNameAsync(userName);
        }

        public Task<string> GenerateChangeEmailTokenAsync(IIdentityUser user, string newEmail)
        {
            return base.GenerateChangeEmailTokenAsync(AsUser(user), newEmail);
        }

        public Task<string> GenerateChangePhoneNumberTokenAsync(IIdentityUser user, string phoneNumber)
        {
            return base.GenerateChangePhoneNumberTokenAsync(AsUser(user), phoneNumber);
        }

        public Task<string> GenerateConcurrencyStampAsync(IIdentityUser user)
        {
            return base.GenerateConcurrencyStampAsync(AsUser(user));
        }

        public Task<string> GenerateEmailConfirmationTokenAsync(IIdentityUser user)
        {
            return base.GenerateEmailConfirmationTokenAsync(AsUser(user));
        }

        public Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(IIdentityUser user, int number)
        {
            return base.GenerateNewTwoFactorRecoveryCodesAsync(AsUser(user), number);
        }

        public Task<string> GeneratePasswordResetTokenAsync(IIdentityUser user)
        {
            return base.GeneratePasswordResetTokenAsync(AsUser(user));
        }

        public Task<string> GenerateTwoFactorTokenAsync(IIdentityUser user, string tokenProvider)
        {
            return base.GenerateTwoFactorTokenAsync(AsUser(user), tokenProvider);
        }

        public Task<string> GenerateUserTokenAsync(IIdentityUser user, string tokenProvider, string purpose)
        {
            return base.GenerateUserTokenAsync(AsUser(user), tokenProvider, purpose);
        }

        public Task<int> GetAccessFailedCountAsync(IIdentityUser user)
        {
            return base.GetAccessFailedCountAsync(AsUser(user));
        }

        public Task<string> GetAuthenticationTokenAsync(IIdentityUser user, string loginProvider, string tokenName)
        {
            return base.GetAuthenticationTokenAsync(AsUser(user), loginProvider, tokenName);
        }

        public Task<string> GetAuthenticatorKeyAsync(IIdentityUser user)
        {
            return base.GetAuthenticatorKeyAsync(AsUser(user));
        }

        public Task<IList<Claim>> GetClaimsAsync(IIdentityUser user)
        {
            return base.GetClaimsAsync(AsUser(user));
        }

        public Task<string> GetEmailAsync(IIdentityUser user)
        {
            return base.GetEmailAsync(AsUser(user));
        }

        public Task<bool> GetLockoutEnabledAsync(IIdentityUser user)
        {
            return base.GetLockoutEnabledAsync(AsUser(user));
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(IIdentityUser user)
        {
            return base.GetLockoutEndDateAsync(AsUser(user));
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(IIdentityUser user)
        {
            return base.GetLoginsAsync(AsUser(user));
        }

        public Task<string> GetPhoneNumberAsync(IIdentityUser user)
        {
            return base.GetPhoneNumberAsync(AsUser(user));
        }

        public Task<IList<string>> GetRolesAsync(IIdentityUser user)
        {
            return base.GetRolesAsync(AsUser(user));
        }

        public Task<string> GetSecurityStampAsync(IIdentityUser user)
        {
            return base.GetSecurityStampAsync(AsUser(user));
        }

        public Task<bool> GetTwoFactorEnabledAsync(IIdentityUser user)
        {
            return base.GetTwoFactorEnabledAsync(AsUser(user));
        }

        public async Task<IIdentityUser> GetIdentityUserAsync(ClaimsPrincipal principal)
        {
            return await base.GetUserAsync(principal);
        }

        public Task<string> GetUserIdAsync(IIdentityUser user)
        {
            return base.GetUserIdAsync(AsUser(user));
        }

        public Task<string> GetUserNameAsync(IIdentityUser user)
        {
            return base.GetUserNameAsync(AsUser(user));
        }

        public async Task<IList<IIdentityUser>> GetIdentityUsersForClaimAsync(Claim claim)
        {
            var list = await base.GetUsersForClaimAsync(claim);
            return list.Select(r => r as IIdentityUser).ToList();
        }

        public async Task<IList<IIdentityUser>> GetIdentityUsersInRoleAsync(string roleName)
        {
            var list = await base.GetUsersInRoleAsync(roleName);
            return list.Select(r => r as IIdentityUser).ToList();
        }

        public Task<IList<string>> GetValidTwoFactorProvidersAsync(IIdentityUser user)
        {
            return base.GetValidTwoFactorProvidersAsync(AsUser(user));
        }

        public Task<bool> HasPasswordAsync(IIdentityUser user)
        {
            return base.HasPasswordAsync(AsUser(user));
        }

        public Task<bool> IsEmailConfirmedAsync(IIdentityUser user)
        {
            return base.IsEmailConfirmedAsync(AsUser(user));
        }

        public Task<bool> IsInRoleAsync(IIdentityUser user, string role)
        {
            return base.IsInRoleAsync(AsUser(user), role);
        }

        public Task<bool> IsLockedOutAsync(IIdentityUser user)
        {
            return base.IsLockedOutAsync(AsUser(user));
        }

        public Task<bool> IsPhoneNumberConfirmedAsync(IIdentityUser user)
        {
            return base.IsPhoneNumberConfirmedAsync(AsUser(user));
        }

        public Task<IdentityResult> RedeemTwoFactorRecoveryCodeAsync(IIdentityUser user, string code)
        {
            return base.RedeemTwoFactorRecoveryCodeAsync(AsUser(user), code);
        }

        public Task<IdentityResult> RemoveAuthenticationTokenAsync(IIdentityUser user, string loginProvider, string tokenName)
        {
            return base.RemoveAuthenticationTokenAsync(AsUser(user), loginProvider, tokenName);
        }

        public Task<IdentityResult> RemoveClaimAsync(IIdentityUser user, Claim claim)
        {
            return base.RemoveClaimAsync(AsUser(user), claim);
        }

        public Task<IdentityResult> RemoveClaimsAsync(IIdentityUser user, IEnumerable<Claim> claims)
        {
            return base.RemoveClaimsAsync(AsUser(user), claims);
        }

        public Task<IdentityResult> RemoveFromRoleAsync(IIdentityUser user, string role)
        {
            return base.RemoveFromRoleAsync(AsUser(user), role);
        }

        public Task<IdentityResult> RemoveFromRolesAsync(IIdentityUser user, IEnumerable<string> roles)
        {
            return base.RemoveFromRolesAsync(AsUser(user), roles);
        }

        public Task<IdentityResult> RemoveLoginAsync(IIdentityUser user, string loginProvider, string providerKey)
        {
            return base.RemoveLoginAsync(AsUser(user), loginProvider, providerKey);
        }

        public Task<IdentityResult> RemovePasswordAsync(IIdentityUser user)
        {
            return base.RemovePasswordAsync(AsUser(user));
        }

        public Task<IdentityResult> ReplaceClaimAsync(IIdentityUser user, Claim claim, Claim newClaim)
        {
            return base.ReplaceClaimAsync(AsUser(user), claim, newClaim);
        }

        public Task<IdentityResult> ResetAccessFailedCountAsync(IIdentityUser user)
        {
            return base.ResetAccessFailedCountAsync(AsUser(user));
        }

        public Task<IdentityResult> ResetAuthenticatorKeyAsync(IIdentityUser user)
        {
            return base.ResetAuthenticatorKeyAsync(AsUser(user));
        }

        public Task<IdentityResult> ResetPasswordAsync(IIdentityUser user, string token, string newPassword)
        {
            return base.ResetPasswordAsync(AsUser(user), token, newPassword);
        }

        public Task<IdentityResult> SetAuthenticationTokenAsync(IIdentityUser user, string loginProvider, string tokenName, string tokenValue)
        {
            return base.SetAuthenticationTokenAsync(AsUser(user), loginProvider, tokenName, tokenValue);
        }

        public Task<IdentityResult> SetEmailAsync(IIdentityUser user, string email)
        {
            return base.SetEmailAsync(AsUser(user), email);
        }

        public Task<IdentityResult> SetLockoutEnabledAsync(IIdentityUser user, bool enabled)
        {
            return base.SetLockoutEnabledAsync(AsUser(user), enabled);
        }

        public Task<IdentityResult> SetLockoutEndDateAsync(IIdentityUser user, DateTimeOffset? lockoutEnd)
        {
            return base.SetLockoutEndDateAsync(AsUser(user), lockoutEnd);
        }

        public Task<IdentityResult> SetPhoneNumberAsync(IIdentityUser user, string phoneNumber)
        {
            return base.SetPhoneNumberAsync(AsUser(user), phoneNumber);
        }

        public Task<IdentityResult> SetTwoFactorEnabledAsync(IIdentityUser user, bool enabled)
        {
            return base.SetTwoFactorEnabledAsync(AsUser(user), enabled);
        }

        public Task<IdentityResult> UpdateAsync(IIdentityUser user)
        {
            return base.UpdateAsync(AsUser(user));
        }

        public Task UpdateNormalizedEmailAsync(IIdentityUser user)
        {
            return base.UpdateNormalizedEmailAsync(AsUser(user));
        }

        public Task UpdateNormalizedUserNameAsync(IIdentityUser user)
        {
            return base.UpdateNormalizedUserNameAsync(AsUser(user));
        }

        public Task<IdentityResult> UpdateSecurityStampAsync(IIdentityUser user)
        {
            return base.UpdateSecurityStampAsync(AsUser(user));
        }

        public Task<bool> VerifyChangePhoneNumberTokenAsync(IIdentityUser user, string token, string phoneNumber)
        {
            return base.VerifyChangePhoneNumberTokenAsync(AsUser(user), token, phoneNumber);
        }

        public Task<bool> VerifyTwoFactorTokenAsync(IIdentityUser user, string tokenProvider, string token)
        {
            return base.VerifyTwoFactorTokenAsync(AsUser(user), tokenProvider, token);
        }

        public Task<bool> VerifyUserTokenAsync(IIdentityUser user, string tokenProvider, string purpose, string token)
        {
            return base.VerifyUserTokenAsync(AsUser(user), tokenProvider, purpose, token);
        }

        /// <summary>
        /// 根据身份证查找用户,手机号码
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<IIdentityUser?> FindIdentityUserByPhoneNumberAsync(string phoneNumber)
        {
            return await Task.FromResult(Users.SingleOrDefault(u => u.PhoneNumber == phoneNumber));
        }

        /// <summary>
        /// 根据身份证查找用户,身份证号码唯一有效
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        public async Task<IIdentityUser?> FindIdentityUserByIdCardAsync(string idCard)
        {
            idCard = idCard.ToUpperInvariant();
            return await Task.FromResult(Users.SingleOrDefault(u => u.IdCard == idCard));
        }

        /// <summary>
        /// 获取totp算法两阶段登录的二维码内容
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<string> GetTotpTwoFactorQrcodeAsync(string userId)
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
        public async Task<string> GenerateLoginCaptchaAsync(string userId)
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
        public async Task<bool> IsValidLoginCaptchaAsync(string userId, string catpcha)
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
        public async Task<string> GenerateLoginCaptchaAsync(IIdentityUser user)
        {
            return await this.GenerateUserTokenAsync(AsUser(user), ShashlikIdentityConsts.CaptchaTokenProvider,
                ShashlikIdentityConsts.LoginPurpose);
        }

        /// <summary>
        /// 生成用于登录的验证码,使用Captcha
        /// </summary>
        /// <param name="user"></param>
        /// <param name="catpcha"></param>
        /// <returns></returns>
        public async Task<bool> IsValidLoginCaptchaAsync(IIdentityUser user, string catpcha)
        {
            return await this.VerifyUserTokenAsync(AsUser(user), ShashlikIdentityConsts.CaptchaTokenProvider,
                ShashlikIdentityConsts.LoginPurpose, catpcha);
        }

        public async Task<SignInResult> CheckPasswordSignInAsync(IIdentityUser identityUser, string password, bool lockoutOnFailure)
        {
            var user = identityUser as TUser;
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var error = await PreSignInCheck(user);
            if (error != null)
                return error;

            // 密码正确
            if (await CheckPasswordAsync(user, password))
            {
                await ResetAccessFailedCountAsync(user);
                if (await IsTwoFactorEnabled(user))
                    // 需要两阶段登录
                    return SignInResult.TwoFactorRequired;
                return SignInResult.Success;
            }

            if (SupportsUserLockout && lockoutOnFailure)
            {
                await AccessFailedAsync(user);
                if (await IsLockedOutAsync(user))
                    return SignInResult.LockedOut;
            }

            return SignInResult.Failed;
        }

        #region private

        private async Task<bool> CanSignInAsync(TUser user)
        {
            if (Options.SignIn.RequireConfirmedEmail && !(await IsEmailConfirmedAsync(user)))
                return false;
            if (Options.SignIn.RequireConfirmedPhoneNumber && !(await IsPhoneNumberConfirmedAsync(user)))
                return false;
            if (Options.SignIn.RequireConfirmedAccount && !(await UserConfirmation.IsConfirmedAsync(this, user)))
                return false;
            return true;
        }

        private async Task<SignInResult?> PreSignInCheck(TUser user)
        {
            if (!await CanSignInAsync(user))
                return SignInResult.NotAllowed;

            if (SupportsUserLockout && await IsLockedOutAsync(user))
                return await Task.FromResult(SignInResult.LockedOut);

            return null;
        }


        private async Task<bool> IsTwoFactorEnabled(TUser user)
        {
            return SupportsUserTwoFactor
                   && await GetTwoFactorEnabledAsync(user)
                   && (await GetValidTwoFactorProvidersAsync(user)).Count > 0;
        }

        #endregion
    }
}