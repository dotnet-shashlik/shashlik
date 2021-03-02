using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.Extensions.Options;
using Shashlik.Identity;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 通用的验证码登录,手机号码唯一有效,要求用户必须存在
    /// </summary>
    public class ShashlikCaptchaValidator : IExtensionGrantValidator
    {
        public ShashlikCaptchaValidator(
            IShashlikUserManager userManager,
            IIdentityUserProvider userFinder,
            IOptions<ShashlikIds4IdentityOptions> shashlikIds4IdentityOptions)
        {
            UserManager = userManager;
            IdentityUserFinder = userFinder;
            ShashlikIds4IdentityOptions = shashlikIds4IdentityOptions;
        }

        private IShashlikUserManager UserManager { get; }
        private IIdentityUserProvider IdentityUserFinder { get; }
        private IOptions<ShashlikIds4IdentityOptions> ShashlikIds4IdentityOptions { get; }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var identity = context.Request.Raw.Get("identity");
            var captcha = context.Request.Raw.Get("captcha");
            if (captcha.IsNullOrWhiteSpace())
            {
                context.WriteError(ErrorCodes.TokenError);
                return;
            }

            if (identity.IsNullOrWhiteSpace())
            {
                context.WriteError(ErrorCodes.IdentityError);
                return;
            }

            var user = await IdentityUserFinder.FindByIdentityAsync(identity!,
                ShashlikIds4IdentityOptions.Value.CaptchaSignInSources!, UserManager, context.Request.Raw);
            if (user is null)
            {
                context.WriteError(ErrorCodes.UserNotFound);
                return;
            }

            var sub = await UserManager.GetUserIdAsync(user);
            if (await UserManager.IsValidLoginCaptchaAsync(user, captcha!))
            {
                context.Result = new GrantValidationResult(sub, GrantType);
                return;
            }

            context.WriteError(ErrorCodes.TokenError);
        }

        public string GrantType => ShashlikIds4IdentityConsts.CaptchaGrantType;
    }
}