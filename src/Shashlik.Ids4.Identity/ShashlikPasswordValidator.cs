using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Shashlik.Identity;
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
        public ShashlikPasswordValidator(
            IShashlikUserManager userManager,
            IOptions<ShashlikIds4IdentityOptions> shashlikIds4IdentityOptions,
            IDataProtectionProvider dataProtectionProvider, IIdentityUserFinder identityUserFinder)
        {
            UserManager = userManager;
            ShashlikIds4IdentityOptions = shashlikIds4IdentityOptions;
            DataProtectionProvider = dataProtectionProvider;
            IdentityUserFinder = identityUserFinder;
        }

        private IShashlikUserManager UserManager { get; }
        private IOptions<ShashlikIds4IdentityOptions> ShashlikIds4IdentityOptions { get; }
        private IDataProtectionProvider DataProtectionProvider { get; }
        private IIdentityUserFinder IdentityUserFinder { get; }

        /// <summary>
        /// Validates the resource owner password credential
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var username = context.UserName;
            var password = context.Password;

            if (username.IsNullOrWhiteSpace())
            {
                context.WriteError(ErrorCodes.UserNameOrPasswordError);
                return;
            }

            if (password.IsNullOrWhiteSpace())
            {
                context.WriteError(ErrorCodes.UserNameOrPasswordError);
                return;
            }

            var user = await IdentityUserFinder.FindByIdentityAsync(username,
                ShashlikIds4IdentityOptions.Value.PasswordSignInSources!, UserManager, context.Request.Raw);

            if (user is null)
            {
                context.WriteError(ErrorCodes.UserNameOrPasswordError);
                return;
            }

            {
                var result = await UserManager.CheckPasswordSignInAsync(user, password, true);
                if (result.Succeeded)
                {
                    // 账户密码成功,且未启用两阶段登录
                    var sub = await UserManager.GetUserIdAsync(user);
                    context.Result = new GrantValidationResult(sub, OidcConstants.AuthenticationMethods.Password);
                    return;
                }

                if (result.IsLockedOut)
                {
                    context.WriteError(ErrorCodes.UserLockout);
                    return;
                }

                if (result.IsNotAllowed)
                {
                    context.WriteError(ErrorCodes.NotAllowLogin);
                    return;
                }

                if (result.RequiresTwoFactor)
                {
                    // 如果需要两阶段登录,将用户id和过期时间组装成json让后加密返回给前端(security),前端执行第二阶段登录时传入security
                    var data = new TwoFactorStep1SecurityModel
                    {
                        UserId = user.IdString,
                        Nonce = Guid.NewGuid().ToString("n")
                    };

                    var json = JsonSerializer.Serialize(data);
                    var security = DataProtectionProvider
                        .CreateProtector(ShashlikIds4IdentityConsts.TwoFactorTokenProviderPurpose)
                        .ToTimeLimitedDataProtector()
                        .Protect(json, TimeSpan.FromSeconds(ShashlikIds4IdentityOptions.Value.TwoFactorExpiration));

                    context.Result = new GrantValidationResult
                    {
                        IsError = true,
                        Error = ((int) ErrorCodes.RequiresTwoFactor).ToString(),
                        ErrorDescription = ErrorCodes.RequiresTwoFactor.GetEnumDescription(),
                        CustomResponse = new Dictionary<string, object?>
                        {
                            {"code", (int) ErrorCodes.RequiresTwoFactor},
                            {"message", ErrorCodes.RequiresTwoFactor.GetEnumDescription()},
                            {"security", security}
                        }
                    };

                    return;
                }

                context.WriteError(ErrorCodes.Other);
            }
        }
    }
}