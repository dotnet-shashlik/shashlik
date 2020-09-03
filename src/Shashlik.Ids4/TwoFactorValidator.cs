using Guc.Features.VerifyCode;
using Guc.Utils.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Sbt.Common;
using Sbt.Identity;
using Sbt.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Sbt.Common.Enums;

namespace Sbt.Ids4
{
    /// <summary>
    /// 两部登录
    /// </summary>
    public class TwoFactorValidator : IExtensionGrantValidator
    {

        UserManager userManager { get; }
        SignInManager<Users> signInManager { get; }
        IVerifyCodeFeature verifyCodeFeature { get; }
        public TwoFactorValidator(
            UserManager userManager,
            IVerifyCodeFeature verifyCodeFeature,
            SignInManager<Users> signInManager
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.verifyCodeFeature = verifyCodeFeature;
        }

        public string GrantType => "two_factor";


        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var username = context.Request.Raw.Get("username");
            var password = context.Request.Raw.Get("password")?.ToUpper();
            var code = context.Request.Raw.Get("code");

            if (username.IsNullOrWhiteSpace())
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "用户名不能为空"
                };
                return;
            }
            if (password.IsNullOrWhiteSpace() || password.Length != 32)
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "密码不能为空"
                };
                return;
            }

            if (code.IsNullOrWhiteSpace())
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "短信验证码错误"
                };
                return;
            }

            // 根据用户名和邮件地址查找用户
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
                user = await userManager.FindByEmailAsync(username);
            if (user == null)
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "用户名或密码错误"
                };
                return;
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

            if (!await verifyCodeFeature.IsValid(Enums.SmsSubject.Login.ToString(), user.PhoneNumber, code))
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "短信验证码错误"
                };
                return;
            }


            var result = await signInManager.PasswordSignInAsync(username, password, true, true);
            if (result.Succeeded)
            {
                context.Result = new GrantValidationResult(user.Id.ToString(), GrantType);
            }
            else if (result.IsLockedOut)
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "账户已被锁定"
                };
                return;
            }
            else if (result.IsNotAllowed)
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "不允许登录"
                };
                return;
            }
            else
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "用户名或密码错误"
                };
                return;
            }
        }
    }
}
