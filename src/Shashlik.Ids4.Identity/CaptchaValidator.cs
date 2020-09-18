using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Shashlik.Captcha;
using Shashlik.Identity;
using Shashlik.Identity.Entities;
using Shashlik.Ids4.Identity.Interfaces;
using Shashlik.Utils.Extensions;

// ReSharper disable ClassNeverInstantiated.Global

// ReSharper disable AssignNullToNotNullAttribute

#pragma warning disable 8600

// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 通用的验证码登录,手机号码唯一有效,要求用户必须存在
    /// </summary>
    public class CaptchaValidator : IExtensionGrantValidator
    {
        public CaptchaValidator(ICaptcha captcha, ShashlikUserManager userManager,
            IEnumerable<ICaptchaUserGetter> captchaUserGetters)
        {
            Captcha = captcha;
            UserManager = userManager;
            CaptchaUserGetters = captchaUserGetters;
        }

        private ICaptcha Captcha { get; }
        private ShashlikUserManager UserManager { get; }
        private IEnumerable<ICaptchaUserGetter> CaptchaUserGetters { get; }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var type = context.Request.Raw.Get("type");
            var errorCode = 0;
            var unifier = context.Request.Raw.Get("unifier");
            if (type.IsNullOrWhiteSpace())
                errorCode = ShashlikIds4IdentityConsts.ErrorCodes.TypeError;

            ICaptchaUserGetter captchaUserGetter = null;
            if (errorCode == 0)
                captchaUserGetter = CaptchaUserGetters.FirstOrDefault(r => r.Type == type);
            errorCode = captchaUserGetter?.Validate(unifier) ?? ShashlikIds4IdentityConsts.ErrorCodes.TypeError;

            var code = context.Request.Raw.Get("code");

            if (unifier.IsNullOrWhiteSpace())
                errorCode = ShashlikIds4IdentityConsts.ErrorCodes.PhoneNumberError;
            if (!unifier.IsMatch(Utils.Consts.Regexs.MobilePhoneNumber))
                errorCode = ShashlikIds4IdentityConsts.ErrorCodes.PhoneNumberError;
            if (code.IsNullOrWhiteSpace())
                errorCode = ShashlikIds4IdentityConsts.ErrorCodes.TokenError;
            if (!await Captcha.IsValid(ShashlikIds4IdentityConsts.LoginPurpose, unifier, code))
                errorCode = ShashlikIds4IdentityConsts.ErrorCodes.TokenError;

            Users user = null;
            if (captchaUserGetter != null)
                user = await captchaUserGetter.FindByUnifierAsync(unifier, UserManager, context.Request.Raw);
            if (user == null)
                errorCode = ShashlikIds4IdentityConsts.ErrorCodes.UnifierError;

            if (user != null)
            {
                var sub = await UserManager.GetUserIdAsync(user);
                context.Result = new GrantValidationResult(sub, GrantType);
                return;
            }

            context.Result = new GrantValidationResult
            {
                IsError = true,
                Error = errorCode.ToString()
            };
        }

        public string GrantType => ShashlikIds4IdentityConsts.CaptchaGrantType;
    }
}