using IdentityServer4.Validation;
using System.Threading.Tasks;
using Guc.Utils.Extensions;
using Guc.Utils;
using Sbt.Identity;
using Guc.Features.VerifyCode;
using Sbt.Common;
using Microsoft.AspNetCore.Identity;
using Sbt.Identity.Entities;
using System;
using Microsoft.Extensions.Hosting;

namespace Sbt.Ids4
{
    /*
    * 能用短信登录的一定是手机号码唯一,即phone作为userName
    * 
    * **/

    /// <summary>
    /// 短信登录
    /// </summary>
    public class SmsValidator : IExtensionGrantValidator
    {
        UserManager UserManager { get; }
        IVerifyCodeFeature VerifyCodeService { get; }
        public SmsValidator(
            UserManager userManager,
            IVerifyCodeFeature verifyCodeService
            )
        {
            UserManager = userManager;
            VerifyCodeService = verifyCodeService;
        }

        public string GrantType => "sms";

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
            if (!await VerifyCodeService.IsValid(Enums.SmsSubject.Login.ToString(), phone, code))
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "验证码错误"
                };
                return;
            }

            var user = await UserManager.FindByNameAsync(phone);
            if (user == null)
            {
                if (clientId != Globals.ClientIds.PersonH5)
                {
                    context.Result = new GrantValidationResult
                    {
                        IsError = true,
                        Error = "用户不存在"
                    };
                    return;
                }

                user = new Users
                {
                    UserName = phone,
                    PhoneNumber = phone,
                    PhoneNumberConfirmed = true,
                    Gender = Enums.Gender.Unknown,
                    CreateTime = DateTime.Now.GetLongDate()
                };
                await UserManager.CreateAndAddRole(user, Globals.Roles.Person);
            }

            if (user.LoginDisaled)
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "账户已禁止登录"
                };
                return;
            }

            context.Result = new GrantValidationResult(user.Id.ToString(), GrantType);
        }
    }
}
