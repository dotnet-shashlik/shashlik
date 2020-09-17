using System.Threading.Tasks;
using IdentityServer4.Validation;
using Shashlik.Captcha;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 邮件两步验证
    /// </summary>
    public class EMail2FAValidator : IExtensionGrantValidator
    {
        private readonly ICaptcha _captcha;

        public EMail2FAValidator(ICaptcha captcha)
        {
            _captcha = captcha;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var email = context.Request.Raw.Get("email");
            var code = context.Request.Raw.Get("code");
            var clientId = context.Request.Raw.Get("client_id");
            if (email.IsNullOrWhiteSpace())
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "Email不能为空"
                };
                return;
            }

            if (!email.IsMatch(Utils.Consts.Regexs.Email))
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "Email格式错误"
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

            if (!await _captcha.IsValid(Consts._2FAPurpose, email, code))
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

        public string GrantType => Consts.EMail2FAGrantType;
    }
}