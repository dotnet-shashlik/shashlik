using System.Threading.Tasks;
using IdentityServer4.Validation;
using Shashlik.Captcha;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 手机两部验证
    /// </summary>
    public class Phone2FAValidator : IExtensionGrantValidator
    {
        private readonly ICaptcha _captcha;

        public Phone2FAValidator(ICaptcha captcha)
        {
            _captcha = captcha;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var phone = context.Request.Raw.Get("phone");
            var code = context.Request.Raw.Get("code");
            var clientId = context.Request.Raw.Get("client_id");
            if (phone.IsNullOrWhiteSpace())
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "手机号码不能为空"
                };
                return;
            }

            if (!phone.IsMatch(Utils.Consts.Regexs.MobilePhoneNumber))
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

            if (!await _captcha.IsValid(Consts._2FAPurpose, phone, code))
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "验证码错误"
                };
                return;
            }

            context.Result = new GrantValidationResult();
        }

        public string GrantType => Consts.Phone2FAGrantType;
    }
}