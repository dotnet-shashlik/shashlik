// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Identity;
using Shashlik.Identity.Entities;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// IResourceOwnerPasswordValidator that integrates with ASP.NET Identity.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <seealso cref="IdentityServer4.Validation.IResourceOwnerPasswordValidator" />
    public class PasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly SignInManager<Users> _signInManager;
        private readonly ShashlikUserManager _userManager;
        private readonly IOptions<ShashlikIdentityOptions> _shashlikIdentityOptions;
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly IOptions<Ids4IdentityOptions> _ids4IdentityOptions;

        public PasswordValidator(ShashlikUserManager userManager, IOptions<Ids4IdentityOptions> ids4IdentityOptions,
            IOptions<ShashlikIdentityOptions> shashlikIdentityOptions, IOptions<IdentityOptions> identityOptions,
            SignInManager<Users> signInManager)
        {
            _userManager = userManager;
            _ids4IdentityOptions = ids4IdentityOptions;
            _shashlikIdentityOptions = shashlikIdentityOptions;
            _identityOptions = identityOptions;
            _signInManager = signInManager;
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
            var clientId = context.Request.ClientId;
            var errorCode = 0;

            if (username.IsNullOrWhiteSpace())
                errorCode = -1;
            if (password.IsNullOrWhiteSpace())
                errorCode = -2;

            // 根据用户名和邮件地址查找用户
            var user = await _userManager.FindByNameAsync(username);
            if (user == null && _ids4IdentityOptions.Value.PasswordSignInSources.Contains(Consts.EMailSource))
                user = await _userManager.FindByEmailAsync(username);
            if (user == null && _ids4IdentityOptions.Value.PasswordSignInSources.Contains(Consts.PhoneSource))
                user = await _userManager.FindByPhoneNumberAsync(username);
            if (user == null && _ids4IdentityOptions.Value.PasswordSignInSources.Contains(Consts.IdCardSource))
                user = await _userManager.FindByPhoneNumberAsync(username);
            if (user == null)
                errorCode = -3;

            var result = await _signInManager.PasswordSignInAsync(user, password, true, true);
            if (result.Succeeded)
            {
                var sub = await _userManager.GetUserIdAsync(user);
                context.Result = new GrantValidationResult(sub, OidcConstants.AuthenticationMethods.Password);
            }
            else if (result.IsLockedOut)
            {
                //TODO....
            }
            else if (result.IsNotAllowed)
            {
                //TODO....
            }
            else if (result.RequiresTwoFactor)
            {
                // 需要两步登录
                //TODO....
            }
            else
            {
                //TODO....
            }
        }
    }
}