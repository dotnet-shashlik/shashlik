using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shashlik.Identity;
using Shashlik.Identity.Entities;
using Shashlik.Identity.Options;
using Shashlik.Utils.Extensions;

#pragma warning disable 8600

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 手机两部验证
    /// </summary>
    public class TwoFactorValidator : IExtensionGrantValidator
    {
        private readonly ShashlikUserManager _userManager;
        private readonly IOptions<ShashlikIdentityOptions> _shashlikIdentityOptions;
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly IOptions<ShashlikIds4IdentityOptions> _ids4IdentityOptions;

        public TwoFactorValidator(ShashlikUserManager userManager, IOptions<ShashlikIds4IdentityOptions> ids4IdentityOptions,
            IOptions<ShashlikIdentityOptions> shashlikIdentityOptions, IOptions<IdentityOptions> identityOptions)
        {
            _userManager = userManager;
            _ids4IdentityOptions = ids4IdentityOptions;
            _shashlikIdentityOptions = shashlikIdentityOptions;
            _identityOptions = identityOptions;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            //TODO: username
            var username = context.Request.Raw.Get("username");
            // token: 两步验证的第二部验证码
            var token = context.Request.Raw.Get("token");
            // provider: 两步验证使用什么验证提供类,Captcha(基于Shashlik.Captcha随机验证码)/Phone(默认为totp算法)/Email
            var provider = context.Request.Raw.Get("provider");

            var errorCode = 0;
            if (username.IsNullOrWhiteSpace())
                errorCode = ShashlikIds4IdentityConsts.ErrorCodes.UserNameEmpty;
            if (token.IsNullOrWhiteSpace())
                errorCode = ShashlikIds4IdentityConsts.ErrorCodes.TokenError;
            if (provider.IsNullOrWhiteSpace())
                errorCode = ShashlikIds4IdentityConsts.ErrorCodes.ProviderError;

            Users user = null;
            if (errorCode != 0)
            {
                // 根据用户名和邮件地址查找用户
                user = await _userManager.FindByNameAsync(username);
                if (user == null &&
                    _ids4IdentityOptions.Value.PasswordSignInSources.Contains(ShashlikIds4IdentityConsts.EMailSource))
                    user = await _userManager.FindByEmailAsync(username);
                if (user == null &&
                    _ids4IdentityOptions.Value.PasswordSignInSources.Contains(ShashlikIds4IdentityConsts.PhoneSource))
                    user = await _userManager.FindByPhoneNumberAsync(username);
                if (user == null &&
                    _ids4IdentityOptions.Value.PasswordSignInSources.Contains(ShashlikIds4IdentityConsts.IdCardSource))
                    user = await _userManager.FindByPhoneNumberAsync(username);
                if (user == null)
                    errorCode = ShashlikIds4IdentityConsts.ErrorCodes.UsernameOrPasswordError;
            }

            if (user != null)
            {
                // 验证码provider是否可用
                var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
                if (!providers.Contains(provider))
                    errorCode = ShashlikIds4IdentityConsts.ErrorCodes.ProviderError;
            }

            if (user != null && !await _userManager.VerifyTwoFactorTokenAsync(user, provider, token))
                errorCode = -4;

            if (errorCode != 0)
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = errorCode.ToString(),
                };
                return;
            }

            context.Result = new GrantValidationResult(user!.Id.ToString(), this.GrantType);
        }

        public string GrantType => ShashlikIds4IdentityConsts.TwoFactorGrantType;
    }
}