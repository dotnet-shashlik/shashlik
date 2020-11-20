// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// copy from https://github.com/dotnet/aspnetcore/blob/release/3.1/src/Identity/Core/src/IdentityBuilderExtensions.cs

using System;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Identity.DataProtection;
using Shashlik.Identity.Options;

namespace Shashlik.Identity
{
    public static class Extensions
    {
        /// <summary>
        /// Adds the default token providers used to generate tokens for reset passwords, change email
        /// and change telephone number operations, and for two factor authentication token generation.
        /// </summary>
        /// <param name="builder">The current <see cref="IdentityBuilder"/> instance.</param>
        /// <returns>The current <see cref="IdentityBuilder"/> instance.</returns>
        public static IdentityBuilder AddDefaultTokenProviders(this IdentityBuilder builder)
        {
            var userType = builder.UserType;
            var dataProtectionProviderType = typeof(DataProtectorTokenProvider<>).MakeGenericType(userType);
            var phoneNumberProviderType = typeof(PhoneNumberTokenProvider<>).MakeGenericType(userType);
            var emailTokenProviderType = typeof(EmailTokenProvider<>).MakeGenericType(userType);
            var authenticatorProviderType = typeof(AuthenticatorTokenProvider<>).MakeGenericType(userType);
            return builder.AddTokenProvider(TokenOptions.DefaultProvider, dataProtectionProviderType)
                .AddTokenProvider(TokenOptions.DefaultEmailProvider, emailTokenProviderType)
                .AddTokenProvider(TokenOptions.DefaultPhoneProvider, phoneNumberProviderType)
                .AddTokenProvider(TokenOptions.DefaultAuthenticatorProvider, authenticatorProviderType);
        }

        public static IServiceCollection ConfigureIdentityType<TUser, TRole, TKey>(this IServiceCollection serviceCollection)
        {
            serviceCollection.Configure<IdentityTypeOptions>(r =>
            {
                r.UserType = typeof(TUser);
                r.RoleType = typeof(TRole);
                r.KeyType = typeof(TKey);
            });

            return serviceCollection;
        }

        public static IServiceCollection ConfigureIdentityType(this IServiceCollection serviceCollection, Type userType, Type roleType, Type keyType)
        {
            serviceCollection.Configure<IdentityTypeOptions>(r =>
            {
                r.UserType = userType;
                r.RoleType = roleType;
                r.KeyType = keyType;
            });

            return serviceCollection;
        }
    }
}