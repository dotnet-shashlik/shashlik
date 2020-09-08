using System;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Shashlik.Identity.Entities;
using Shashlik.Utils;
using Shashlik.Utils.Extensions;

namespace Shashlik.Identity.GrantType.Phone
{
    public class PhoneValidator : IExtensionGrantValidator
    {
        public string GrantType { get; } = "phone";
        private ShashlikUserManager UserManager { get; }
        private ShashlikIdentityOptions ShashlikIdentityOptions { get; }
        public PhoneValidator(ShashlikUserManager userManager, IOptions<ShashlikIdentityOptions> shashlikIdentityOptions)
        {
            UserManager = userManager;
            ShashlikIdentityOptions = shashlikIdentityOptions.Value;
            if (ShashlikIdentityOptions.UserProperty.PhoneNumberUnique == false)
            {
                throw new Exception("使用手机认证必须将用户手机号设置为唯一");
            }
            UserManager.RegisterTokenProvider(TokenOptions.DefaultPhoneProvider, new PhoneNumberTokenProvider<Users>());
        }
        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var phone = context.Request.Raw.Get("phone");
            var code = context.Request.Raw.Get("code");
            if (phone.IsNullOrWhiteSpace())
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "手机号码不能为空"
                };
                return;
            }
            if (!phone.IsMatch(Consts.Regexs.MobilePhoneNumber))
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "手机号码格式错误"
                };
                return;
            }
            if (code.IsNullOrWhiteSpace())
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "验证码不能为空"
                };
                return;
            }

            var user = await UserManager.FindByNameAsync(phone);

            if (!await UserManager.VerifyUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, "LOGIN", code))
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "验证码错误"
                };
                return;
            }

            context.Result = new GrantValidationResult(user.Id.ToString(), GrantType);
        }
    }
}
