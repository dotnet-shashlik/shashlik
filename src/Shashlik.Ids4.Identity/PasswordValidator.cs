﻿using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shashlik.Identity;
using Shashlik.Identity.Entities;
using Shashlik.Utils.Extensions;

#pragma warning disable 8600

// ReSharper disable ClassNeverInstantiated.Global

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// IResourceOwnerPasswordValidator that integrates with ASP.NET Identity.
    /// </summary>
    /// <seealso cref="IdentityServer4.Validation.IResourceOwnerPasswordValidator" />
    public class PasswordValidator : IResourceOwnerPasswordValidator
    {
        public PasswordValidator(SignInManager<Users> signInManager, ShashlikUserManager userManager,
            IOptions<ShashlikIds4IdentityOptions> shashlikIds4IdentityOptions, IDataProtector dataProtector)
        {
            SignInManager = signInManager;
            UserManager = userManager;
            ShashlikIds4IdentityOptions = shashlikIds4IdentityOptions;
            DataProtector = dataProtector;
        }

        private SignInManager<Users> SignInManager { get; }
        private ShashlikUserManager UserManager { get; }
        private IOptions<ShashlikIds4IdentityOptions> ShashlikIds4IdentityOptions { get; }
        private IDataProtector DataProtector { get; }

        /// <summary>
        /// Validates the resource owner password credential
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var username = context.UserName;
            var password = context.Password;

            var errorCode = 0;
            var customResponse = new Dictionary<string, object>();
            if (username.IsNullOrWhiteSpace())
                errorCode = ShashlikIds4IdentityConsts.ErrorCodes.UserNameEmpty;
            if (password.IsNullOrWhiteSpace())
                errorCode = ShashlikIds4IdentityConsts.ErrorCodes.PasswordEmpty;

            Users user = null;
            if (errorCode != 0)
            {
                // 根据用户名和邮件地址查找用户
                user = await UserManager.FindByNameAsync(username);
                if (user == null &&
                    ShashlikIds4IdentityOptions.Value.PasswordSignInSources.Contains(ShashlikIds4IdentityConsts
                        .EMailSource))
                    user = await UserManager.FindByEmailAsync(username);
                if (user == null &&
                    ShashlikIds4IdentityOptions.Value.PasswordSignInSources.Contains(ShashlikIds4IdentityConsts
                        .PhoneSource))
                    user = await UserManager.FindByPhoneNumberAsync(username);
                if (user == null &&
                    ShashlikIds4IdentityOptions.Value.PasswordSignInSources.Contains(ShashlikIds4IdentityConsts
                        .IdCardSource))
                    user = await UserManager.FindByPhoneNumberAsync(username);
                if (user == null)
                    errorCode = ShashlikIds4IdentityConsts.ErrorCodes.UsernameOrPasswordError;
            }

            if (user != null)
            {
                var result = await SignInManager.CheckPasswordSignInAsync(user, password, true);
                if (result.Succeeded)
                {
                    var sub = await UserManager.GetUserIdAsync(user);
                    context.Result = new GrantValidationResult(sub, OidcConstants.AuthenticationMethods.Password);
                    return;
                }

                if (result.IsLockedOut)
                    errorCode = ShashlikIds4IdentityConsts.ErrorCodes.UserLockout;
                else if (result.IsNotAllowed)
                    errorCode = ShashlikIds4IdentityConsts.ErrorCodes.NotAllowLogin;
                else if (result.RequiresTwoFactor)
                {
                    errorCode = ShashlikIds4IdentityConsts.ErrorCodes.RequiresTwoFactor;
                    customResponse.Add(",", "");
                }
                else
                    errorCode = ShashlikIds4IdentityConsts.ErrorCodes.Other;
            }


            context.Result = new GrantValidationResult
            {
                IsError = true,
                Error = errorCode.ToString(),
                CustomResponse = customResponse
            };
        }
    }
}