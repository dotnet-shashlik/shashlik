using Guc.Utils.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Sbt.Identity;
using Sbt.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sbt.Ids4
{


    /// <summary>
    /// 密码登录
    /// </summary>
    public class PwdValidator : IExtensionGrantValidator
    {
        UserManager userManager { get; }
        SignInManager<Users> signInManager { get; }
        public PwdValidator(
            UserManager userManager,
            SignInManager<Users> signInManager
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public string GrantType => "pwd";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var username = context.Request.Raw.Get("username");
            var password = context.Request.Raw.Get("password")?.ToUpper();

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

            var result = await signInManager.PasswordSignInAsync(username, password, true, true);
            if (result.Succeeded)
                context.Result = new GrantValidationResult(user.Id.ToString(), GrantType);
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
