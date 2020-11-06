// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4.IdentityInt32
{
    internal class ShashlikUserClaimsFactory<TUser> : IUserClaimsPrincipalFactory<TUser>
        where TUser : class
    {
        public ShashlikUserClaimsFactory(UserManager<TUser> userManager, IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _serviceProvider = serviceProvider;
        }

        private readonly UserManager<TUser> _userManager;

        private readonly IServiceProvider _serviceProvider;


        public async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            // 注意服务注册顺序，identity要先注册
            var prevInstance = _serviceProvider
                .GetServices<IUserClaimsPrincipalFactory<TUser>>()
                .LastOrDefault(r => !r.GetType().IsSubTypeOf(typeof(ShashlikUserClaimsFactory<TUser>)));

            var principal = await prevInstance!.CreateAsync(user);
            var identity = principal.Identities.First();

            if (!identity.HasClaim(r => r.Type == JwtClaimTypes.Role))
            {
                var roles = await _userManager.GetRolesAsync(user);
                identity.AddClaims(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));
            }

            return principal;
        }
    }
}