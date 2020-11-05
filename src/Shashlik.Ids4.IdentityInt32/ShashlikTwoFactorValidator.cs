#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.DataProtection;
using Shashlik.Identity;
using Shashlik.Utils.Extensions;
// ReSharper disable ClassNeverInstantiated.Global

namespace Shashlik.Ids4.IdentityInt32
{
    /// <summary>
    /// 两步验证第二步
    /// </summary>
    public class ShashlikTwoFactorValidator : IShashlikExtensionGrantValidator
    {
        private ShashlikUserManager<Users, int> UserManager { get; }
        private IDataProtectionProvider DataProtectionProvider { get; }

        public ShashlikTwoFactorValidator(
            ShashlikUserManager<Users, int> userManager,
            IDataProtectionProvider dataProtectionProvider)
        {
            UserManager = userManager;
            DataProtectionProvider = dataProtectionProvider;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var security = context.Request.Raw.Get("security");
            ErrorCodes errorCode = 0;
            // token: 两步验证的第二部验证码
            var token = context.Request.Raw.Get("token");
            // provider: 两步验证使用什么验证提供类: Captcha(基于验证码)/Authenticator(标准totp),其他的都可以自行扩展TokenProvider
            var provider = context.Request.Raw.Get("provider");

            if (token.IsNullOrWhiteSpace())
                errorCode = ErrorCodes.TokenError;
            if (provider.IsNullOrWhiteSpace())
                errorCode = ErrorCodes.ProviderError;
            if (security.IsNullOrWhiteSpace())
                errorCode = ErrorCodes.SecurityError;

            TwoFactorStep1SecurityModel? model = null;
            if (errorCode == 0)
            {
                try
                {
                    var json = DataProtectionProvider
                        .CreateProtector(ShashlikIds4IdentityConsts.TwoFactorTokenProviderPurpose)
                        .ToTimeLimitedDataProtector()
                        .Unprotect(security, out var expiration);

                    if (DateTimeOffset.Now > expiration)
                        errorCode = ErrorCodes.SecurityError;
                    else
                        model = JsonSerializer.Deserialize<TwoFactorStep1SecurityModel>(json);
                }
                catch (Exception)
                {
                    errorCode = ErrorCodes.SecurityError;
                }

                if (model == null)
                    errorCode = ErrorCodes.SecurityTimeout;
            }

            Users? user = null;
            if (errorCode == 0 && model != null)
            {
                user = await UserManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    // 验证码provider是否可用
                    var providers = await UserManager.GetValidTwoFactorProvidersAsync(user);
                    if (!providers.Contains(provider))
                        errorCode = ErrorCodes.ProviderError;
                }
            }

            if (errorCode == 0 && user != null && !await UserManager.VerifyTwoFactorTokenAsync(user, provider, token))
                errorCode = ErrorCodes.TokenError;

            if (errorCode != 0)
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    CustomResponse = new Dictionary<string, object>
                    {
                        {"code", (int) errorCode}
                    }
                };
                return;
            }

            context.Result = new GrantValidationResult(user!.Id.ToString(), this.GrantType);
        }

        public string GrantType => ShashlikIds4IdentityConsts.TwoFactorGrantType;
    }
}