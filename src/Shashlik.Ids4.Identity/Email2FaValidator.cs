using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shashlik.Identity;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 手机两部验证
    /// </summary>
    public class Email2FaValidator : IExtensionGrantValidator
    {
        private readonly ShashlikUserManager _userManager;
        private readonly IOptions<ShashlikIdentityOptions> _shashlikIdentityOptions;
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly IOptions<Ids4IdentityOptions> _ids4IdentityOptions;

        public Email2FaValidator(ShashlikUserManager userManager, IOptions<Ids4IdentityOptions> ids4IdentityOptions,
            IOptions<ShashlikIdentityOptions> shashlikIdentityOptions, IOptions<IdentityOptions> identityOptions)
        {
            _userManager = userManager;
            _ids4IdentityOptions = ids4IdentityOptions;
            _shashlikIdentityOptions = shashlikIdentityOptions;
            _identityOptions = identityOptions;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var username = context.Request.Raw.Get("username");
            var token = context.Request.Raw.Get("token");
            var clientId = context.Request.ClientId;

            var errorCode = 0;
            if (username.IsNullOrWhiteSpace())
                errorCode = -1;
            if (token.IsNullOrWhiteSpace())
                errorCode = -2;

            // 根据用户名和邮件地址查找用户
            var user = await _userManager.FindByNameAsync(username);
            if (user == null && _ids4IdentityOptions.Value.PasswordSignInSources.Contains(Consts.EMailSource))
                user = await _userManager.FindByEmailAsync(username);
            if (user == null && _ids4IdentityOptions.Value.PasswordSignInSources.Contains(Consts.PhoneSource))
                user = await _userManager.FindByPhoneNumberAsync(username);
            if (user == null && _ids4IdentityOptions.Value.PasswordSignInSources.Contains(Consts.IdCardSource))
                user = await _userManager.FindByPhoneNumberAsync(username);
            if (user == null)
                errorCode = -3;

            // 两步验证有优化安全行的余地,驾驭clientId到purpose,不同客户端之间不能共享两步验证
            // 使用EmailCaptchaProvider作为邮件两步验证提供类
            if (!await _userManager.VerifyTwoFactorTokenAsync(user,
                Shashlik.Identity.AspNetCore.Consts.EmailCaptchaProvider, token))
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

            context.Result = new GrantValidationResult(user.Id.ToString(), this.GrantType);
        }

        public string GrantType => Consts.Email2FaGrantType;
    }
}