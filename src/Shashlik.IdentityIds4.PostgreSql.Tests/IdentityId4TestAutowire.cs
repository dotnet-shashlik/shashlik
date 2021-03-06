﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;

namespace Shashlik.IdentityIds4.PostgreSql.Tests
{
    public class IdentityId4TestAutowire: IServiceAutowire
    {
        public void Configure(IKernelServices kernelServices)
        {
            kernelServices.Services.AddAuthentication()
                .AddCookie("Cookie")
                .AddCookie(IdentityConstants.ApplicationScheme)
                .AddCookie(IdentityConstants.TwoFactorUserIdScheme)
                .AddCookie(IdentityConstants.ExternalScheme)
                .AddIdentityServerAuthentication();
        }
    }
}