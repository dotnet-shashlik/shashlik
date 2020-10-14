using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.Extensions.Options;
using Shashlik.Captcha;
using Shashlik.Identity;
using Shashlik.Identity.Entities;
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
    public class ShashlikCaptchaValidator : IExtensionGrantValidator
    {
        public ShashlikCaptchaValidator(ShashlikUserManager userManager,
            IIdentityUserFinder userFinder)
        {
            UserManager = userManager;
            IdentityUserFinder = userFinder;
        }

        private ShashlikUserManager UserManager { get; }
        private IIdentityUserFinder IdentityUserFinder { get; }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            ErrorCodes errorCode = 0;
            var identity = context.Request.Raw.Get("identity");
            var captcha = context.Request.Raw.Get("captcha");
            if (captcha.IsNullOrWhiteSpace())
                errorCode = ErrorCodes.TokenError;
            if (identity.IsNullOrWhiteSpace())
                errorCode = ErrorCodes.IdentityError;

            Users user = await IdentityUserFinder.FindByUnifierAsync(identity, UserManager, context.Request.Raw);
            if (user == null)
                errorCode = ErrorCodes.UserNotFound;

            if (user != null)
            {
                var sub = await UserManager.GetUserIdAsync(user);
                if (await UserManager.IsValidLoginCaptcha(user, captcha))
                {
                    context.Result = new GrantValidationResult(sub, GrantType);
                    return;
                }
                else
                    errorCode = ErrorCodes.TokenError;
            }

            context.Result = new GrantValidationResult
            {
                IsError = true,
                CustomResponse = new Dictionary<string, object>
                {
                    {"code", (int) errorCode}
                }
            };
        }

        public string GrantType => ShashlikIds4IdentityConsts.CaptchaGrantType;
    }
}