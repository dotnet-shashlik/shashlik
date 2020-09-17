// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// IResourceOwnerPasswordValidator that integrates with ASP.NET Identity.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <seealso cref="IdentityServer4.Validation.IResourceOwnerPasswordValidator" />
    public class PasswordValidator<TUser> : IResourceOwnerPasswordValidator
        where TUser : class
    {
        private readonly SignInManager<TUser> _signInManager;
        private readonly UserManager<TUser> _userManager;
        private readonly ILogger<PasswordValidator<TUser>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceOwnerPasswordValidator{TUser}"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="logger">The logger.</param>
        public PasswordValidator(
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            ILogger<PasswordValidator<TUser>> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        /// Validates the resource owner password credential
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var username = context.UserName;
            var password = context.Password;

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
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                user = await _userManager.FindByEmailAsync(username);

            if (user == null)
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "用户名或密码错误"
                };
                return;
            }

            var result = await _signInManager.PasswordSignInAsync(username, password, true, true);
            if (result.Succeeded)
            {
                var sub = await _userManager.GetUserIdAsync(user);
                context.Result = new GrantValidationResult(sub, OidcConstants.AuthenticationMethods.Password);
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
