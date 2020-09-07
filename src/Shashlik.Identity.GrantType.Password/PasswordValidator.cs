using System;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Shashlik.Identity.Entities;
using Shashlik.Utils.Extensions;

namespace Shashlik.Identity.GrantType.Password
{
    public class PasswordValidator : IExtensionGrantValidator
    {
        public string GrantType { get; } = "password";
        private ShashlikUserManager UserManager { get; }
        private SignInManager<Users> SignInManager { get; }
        
        public PasswordValidator(ShashlikUserManager userManager, SignInManager<Users> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var username = context.Request.Raw.Get("username");
            var password = context.Request.Raw.Get("password");

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
            var user = await UserManager.FindByNameAsync(username);
            if (user == null)
                user = await UserManager.FindByEmailAsync(username);

            if (user == null)
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "用户名或密码错误"
                };
                return;
            }

            var result = await SignInManager.PasswordSignInAsync(username, password, true, true);
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
