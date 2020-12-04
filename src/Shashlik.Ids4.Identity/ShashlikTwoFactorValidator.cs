using System;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.DataProtection;
using Shashlik.Identity;
using Shashlik.Utils.Extensions;
// ReSharper disable ConditionIsAlwaysTrueOrFalse

// ReSharper disable ClassNeverInstantiated.Global

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 两步验证第二步
    /// </summary>
    public class ShashlikTwoFactorValidator : IExtensionGrantValidator
    {
        private IShashlikUserManager UserManager { get; }
        private IDataProtectionProvider DataProtectionProvider { get; }

        public ShashlikTwoFactorValidator(
            IShashlikUserManager userManager,
            IDataProtectionProvider dataProtectionProvider)
        {
            UserManager = userManager;
            DataProtectionProvider = dataProtectionProvider;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            // security第一步登录生成的userid和随机值的json字符串加密数据
            var security = context.Request.Raw.Get("security");
            // token: 两步验证的第二部验证码
            var token = context.Request.Raw.Get("token");
            // provider: 两步验证使用什么验证提供类: Captcha(基于验证码)/Authenticator(标准totp),其他的都可以自行扩展TokenProvider
            var provider = context.Request.Raw.Get("provider");

            if (token.IsNullOrWhiteSpace())
            {
                context.WriteError(ErrorCodes.TokenError);
                return;
            }

            if (provider.IsNullOrWhiteSpace())
            {
                context.WriteError(ErrorCodes.ProviderError);
                return;
            }

            if (security.IsNullOrWhiteSpace())
            {
                context.WriteError(ErrorCodes.SecurityError);
                return;
            }

            TwoFactorStep1SecurityModel? model;

            try
            {
                var json = DataProtectionProvider
                    .CreateProtector(ShashlikIds4IdentityConsts.TwoFactorTokenProviderPurpose)
                    .ToTimeLimitedDataProtector()
                    .Unprotect(security!, out _);

                model = JsonSerializer.Deserialize<TwoFactorStep1SecurityModel>(json);
            }
            catch (Exception)
            {
                context.WriteError(ErrorCodes.SecurityError);
                return;
            }

            if (model is null)
            {
                context.WriteError(ErrorCodes.SecurityError);
                return;
            }

            var user = await UserManager.FindIdentityUserByIdAsync(model.UserId);
            if (user != null)
            {
                // 验证码provider是否可用
                var providers = await UserManager.GetValidTwoFactorProvidersAsync(user);
                if (!providers.Contains(provider!))
                {
                    context.WriteError(ErrorCodes.ProviderError);
                    return;
                }
            }
            else
            {
                context.WriteError(ErrorCodes.SecurityError);
                return;
            }

            if (!await UserManager.VerifyTwoFactorTokenAsync(user, provider!, token!))
            {
                context.WriteError(ErrorCodes.TokenError);
                return;
            }

            context.Result = new GrantValidationResult(user.IdString, this.GrantType);
        }

        public string GrantType => ShashlikIds4IdentityConsts.TwoFactorGrantType;
    }
}