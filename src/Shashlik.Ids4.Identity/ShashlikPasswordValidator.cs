using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shashlik.Identity;
using Shashlik.Identity.Entities;
using Shashlik.Utils.Extensions;

// ReSharper disable ClassNeverInstantiated.Global

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// IResourceOwnerPasswordValidator that integrates with ASP.NET Identity.
    /// </summary>
    /// <seealso cref="IdentityServer4.Validation.IResourceOwnerPasswordValidator" />
    public class ShashlikPasswordValidator : IResourceOwnerPasswordValidator
    {
        public ShashlikPasswordValidator(SignInManager<Users> signInManager, ShashlikUserManager userManager,
            IOptions<ShashlikIds4IdentityOptions> shashlikIds4IdentityOptions,
            IDataProtectionProvider dataProtectionProvider)
        {
            SignInManager = signInManager;
            UserManager = userManager;
            ShashlikIds4IdentityOptions = shashlikIds4IdentityOptions;
            DataProtectionProvider = dataProtectionProvider;
        }

        private SignInManager<Users> SignInManager { get; }
        private ShashlikUserManager UserManager { get; }
        private IOptions<ShashlikIds4IdentityOptions> ShashlikIds4IdentityOptions { get; }
        private IDataProtectionProvider DataProtectionProvider { get; }

        /// <summary>
        /// Validates the resource owner password credential
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var username = context.UserName;
            var password = context.Password;

            ErrorCodes errorCode = 0;
            var customResponse = new Dictionary<string, object>();
            if (username.IsNullOrWhiteSpace())
                errorCode = ErrorCodes.UsernameOrPasswordError;
            if (password.IsNullOrWhiteSpace())
                errorCode = ErrorCodes.UsernameOrPasswordError;

            Users? user = null;
            if (errorCode != 0)
            {
                // 根据用户名和邮件地址查找用户
                user = await UserManager.FindByNameAsync(username);
                if (user == null &&
                    ShashlikIds4IdentityOptions.Value.PasswordSignInSources.Contains(ShashlikIds4IdentityConsts
                        .EMailSource))
                    user = await UserManager.FindByEmailAsync(username);
                if (user == null &&
                    ShashlikIds4IdentityOptions.Value.PasswordSignInSources.Contains(ShashlikIds4IdentityConsts
                        .PhoneSource))
                    user = await UserManager.FindByPhoneNumberAsync(username);
                if (user == null &&
                    ShashlikIds4IdentityOptions.Value.PasswordSignInSources.Contains(ShashlikIds4IdentityConsts
                        .IdCardSource))
                    user = await UserManager.FindByIdCardAsync(username);
                if (user == null)
                    errorCode = ErrorCodes.UsernameOrPasswordError;
            }

            if (user != null)
            {
                var result = await SignInManager.CheckPasswordSignInAsync(user, password, true);
                if (result.Succeeded)
                {
                    var sub = await UserManager.GetUserIdAsync(user);
                    context.Result = new GrantValidationResult(sub, OidcConstants.AuthenticationMethods.Password);
                    return;
                }

                if (result.IsLockedOut)
                    errorCode = ErrorCodes.UserLockout;
                else if (result.IsNotAllowed)
                    errorCode = ErrorCodes.NotAllowLogin;
                else if (result.RequiresTwoFactor)
                {
                    errorCode = ErrorCodes.RequiresTwoFactor;

                    // 如果需要两阶段登录,将用户id和过期时间组装成json让后加密返回给前端(security),前端执行第二阶段登录时传入security
                    var data = new TwoFactorStep1SecurityModel
                    {
                        UserId = user.Id.ToString(),
                        CreateTime = DateTime.Now.GetLongDate(),
                        Nonce = Guid.NewGuid().ToString("n")
                    };

                    var json = JsonSerializer.Serialize(data);
                    var security = DataProtectionProvider
                        .CreateProtector(ShashlikIds4IdentityConsts.TwoFactorTokenProviderPurpose)
                        .ToTimeLimitedDataProtector()
                        .Protect(json, TimeSpan.FromSeconds(ShashlikIds4IdentityOptions.Value.TwoFactorExpiration));

                    customResponse.Add("security", security);
                }
                else
                    errorCode = ErrorCodes.Other;
            }

            customResponse.Add("code", (int) errorCode);
            context.Result = new GrantValidationResult
            {
                IsError = true,
                CustomResponse = customResponse
            };
        }
    }
}