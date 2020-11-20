using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Shashlik.Identity
{
    public interface IShashlikUserManager
    {
        ILookupNormalizer KeyNormalizer { get; set; }
        IdentityErrorDescriber ErrorDescriber { get; set; }
        IdentityOptions Options { get; set; }
        bool SupportsUserAuthenticationTokens { get; }

        bool SupportsUserAuthenticatorKey { get; }

        bool SupportsUserTwoFactorRecoveryCodes { get; }

        bool SupportsUserPassword { get; }

        bool SupportsUserSecurityStamp { get; }

        bool SupportsUserRole { get; }

        bool SupportsUserLogin { get; }

        bool SupportsUserEmail { get; }

        bool SupportsUserPhoneNumber { get; }

        bool SupportsUserClaim { get; }

        bool SupportsUserLockout { get; }

        bool SupportsUserTwoFactor { get; }

        bool SupportsQueryableUsers { get; }

        Task<IdentityResult> AccessFailedAsync(IIdentityUser user);

        Task<IdentityResult> AddClaimAsync(IIdentityUser user, Claim claim);

        Task<IdentityResult> AddClaimsAsync(IIdentityUser user, IEnumerable<Claim> claims);

        Task<IdentityResult> AddLoginAsync(IIdentityUser user, UserLoginInfo login);

        Task<IdentityResult> AddPasswordAsync(IIdentityUser user, string password);

        Task<IdentityResult> AddToRoleAsync(IIdentityUser user, string role);

        Task<IdentityResult> AddToRolesAsync(IIdentityUser user, IEnumerable<string> roles);

        Task<IdentityResult> ChangeEmailAsync(IIdentityUser user, string newEmail, string token);

        Task<IdentityResult> ChangePasswordAsync(IIdentityUser user, string currentPassword, string newPassword);

        Task<IdentityResult> ChangePhoneNumberAsync(IIdentityUser user, string phoneNumber, string token);

        Task<bool> CheckPasswordAsync(IIdentityUser user, string password);

        Task<IdentityResult> ConfirmEmailAsync(IIdentityUser user, string token);

        Task<int> CountRecoveryCodesAsync(IIdentityUser user);

        Task<IdentityResult> CreateAsync(IIdentityUser user);


        Task<IdentityResult> CreateAsync(IIdentityUser user, string password);


        Task<byte[]> CreateSecurityTokenAsync(IIdentityUser user);


        Task<IdentityResult> DeleteAsync(IIdentityUser user);


        void Dispose();


        Task<IIdentityUser> FindIdentityUserByEmailAsync(string email);


        Task<IIdentityUser> FindIdentityUserByIdAsync(string userId);


        Task<IIdentityUser> FindIdentityUserByLoginAsync(string loginProvider, string providerKey);


        Task<IIdentityUser> FindIdentityUserByNameAsync(string userName);


        Task<string> GenerateChangeEmailTokenAsync(IIdentityUser user, string newEmail);


        Task<string> GenerateChangePhoneNumberTokenAsync(IIdentityUser user, string phoneNumber);


        Task<string> GenerateConcurrencyStampAsync(IIdentityUser user);


        Task<string> GenerateEmailConfirmationTokenAsync(IIdentityUser user);


        string GenerateNewAuthenticatorKey();


        Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(IIdentityUser user, int number);


        Task<string> GeneratePasswordResetTokenAsync(IIdentityUser user);


        Task<string> GenerateTwoFactorTokenAsync(IIdentityUser user, string tokenProvider);


        Task<string> GenerateUserTokenAsync(IIdentityUser user, string tokenProvider, string purpose);


        Task<int> GetAccessFailedCountAsync(IIdentityUser user);


        Task<string> GetAuthenticationTokenAsync(IIdentityUser user, string loginProvider, string tokenName);


        Task<string> GetAuthenticatorKeyAsync(IIdentityUser user);


        Task<IList<Claim>> GetClaimsAsync(IIdentityUser user);


        Task<string> GetEmailAsync(IIdentityUser user);


        Task<bool> GetLockoutEnabledAsync(IIdentityUser user);


        Task<DateTimeOffset?> GetLockoutEndDateAsync(IIdentityUser user);


        Task<IList<UserLoginInfo>> GetLoginsAsync(IIdentityUser user);


        Task<string> GetPhoneNumberAsync(IIdentityUser user);


        Task<IList<string>> GetRolesAsync(IIdentityUser user);


        Task<string> GetSecurityStampAsync(IIdentityUser user);


        Task<bool> GetTwoFactorEnabledAsync(IIdentityUser user);


        Task<IIdentityUser> GetIdentityUserAsync(ClaimsPrincipal principal);


        string GetUserId(ClaimsPrincipal principal);


        Task<string> GetUserIdAsync(IIdentityUser user);


        string GetUserName(ClaimsPrincipal principal);

        Task<string> GetUserNameAsync(IIdentityUser user);


        Task<IList<IIdentityUser>> GetIdentityUsersForClaimAsync(Claim claim);


        Task<IList<IIdentityUser>> GetIdentityUsersInRoleAsync(string roleName);


        Task<IList<string>> GetValidTwoFactorProvidersAsync(IIdentityUser user);


        Task<bool> HasPasswordAsync(IIdentityUser user);


        Task<bool> IsEmailConfirmedAsync(IIdentityUser user);


        Task<bool> IsInRoleAsync(IIdentityUser user, string role);


        Task<bool> IsLockedOutAsync(IIdentityUser user);


        Task<bool> IsPhoneNumberConfirmedAsync(IIdentityUser user);


        string NormalizeEmail(string email);


        string NormalizeName(string name);


        Task<IdentityResult> RedeemTwoFactorRecoveryCodeAsync(IIdentityUser user, string code);


        Task<IdentityResult> RemoveAuthenticationTokenAsync(IIdentityUser user, string loginProvider, string tokenName);


        Task<IdentityResult> RemoveClaimAsync(IIdentityUser user, Claim claim);


        Task<IdentityResult> RemoveClaimsAsync(IIdentityUser user, IEnumerable<Claim> claims);


        Task<IdentityResult> RemoveFromRoleAsync(IIdentityUser user, string role);


        Task<IdentityResult> RemoveFromRolesAsync(IIdentityUser user, IEnumerable<string> roles);


        Task<IdentityResult> RemoveLoginAsync(IIdentityUser user, string loginProvider, string providerKey);


        Task<IdentityResult> RemovePasswordAsync(IIdentityUser user);


        Task<IdentityResult> ReplaceClaimAsync(IIdentityUser user, Claim claim, Claim newClaim);


        Task<IdentityResult> ResetAccessFailedCountAsync(IIdentityUser user);


        Task<IdentityResult> ResetAuthenticatorKeyAsync(IIdentityUser user);


        Task<IdentityResult> ResetPasswordAsync(IIdentityUser user, string token, string newPassword);


        Task<IdentityResult> SetAuthenticationTokenAsync(IIdentityUser user, string loginProvider, string tokenName, string tokenValue);


        Task<IdentityResult> SetEmailAsync(IIdentityUser user, string email);


        Task<IdentityResult> SetLockoutEnabledAsync(IIdentityUser user, bool enabled);


        Task<IdentityResult> SetLockoutEndDateAsync(IIdentityUser user, DateTimeOffset? lockoutEnd);


        Task<IdentityResult> SetPhoneNumberAsync(IIdentityUser user, string phoneNumber);


        Task<IdentityResult> SetTwoFactorEnabledAsync(IIdentityUser user, bool enabled);


        Task<IdentityResult> UpdateAsync(IIdentityUser user);


        Task UpdateNormalizedEmailAsync(IIdentityUser user);


        Task UpdateNormalizedUserNameAsync(IIdentityUser user);


        Task<IdentityResult> UpdateSecurityStampAsync(IIdentityUser user);


        Task<bool> VerifyChangePhoneNumberTokenAsync(IIdentityUser user, string token, string phoneNumber);

        Task<bool> VerifyTwoFactorTokenAsync(IIdentityUser user, string tokenProvider, string token);

        Task<bool> VerifyUserTokenAsync(IIdentityUser user, string tokenProvider, string purpose, string token);

        /// <summary>
        /// 根据身份证查找用户,手机号码
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        Task<IIdentityUser> FindIdentityUserByPhoneNumberAsync(string phoneNumber);

        /// <summary>
        /// 根据身份证查找用户,身份证号码唯一有效
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        Task<IIdentityUser> FindIdentityUserByIdCardAsync(string idCard);

        /// <summary>
        /// 获取totp算法两阶段登录的二维码内容
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<string> GetTotpTwoFactorQrcode(string userId);

        /// <summary>
        /// 生成用于登录的验证码,使用Captcha
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<string> GenerateLoginCaptcha(string userId);

        /// <summary>
        /// 生成用于登录的验证码,使用Captcha
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="catpcha"></param>
        /// <returns></returns>
        Task<bool> IsValidLoginCaptcha(string userId, string catpcha);

        /// <summary>
        /// 生成用于登录的验证码,使用Captcha
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<string> GenerateLoginCaptcha(IIdentityUser user);

        /// <summary>
        /// 生成用于登录的验证码,使用Captcha
        /// </summary>
        /// <param name="user"></param>
        /// <param name="catpcha"></param>
        /// <returns></returns>
        Task<bool> IsValidLoginCaptcha(IIdentityUser user, string catpcha);

        /// <summary>
        /// 检查密码并获取登录结果
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="lockoutOnFailure"></param>
        /// <returns></returns>
        Task<SignInResult> CheckPasswordSignInAsync(IIdentityUser user, string password, bool lockoutOnFailure);
    }
}