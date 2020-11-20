using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// ReSharper disable CheckNamespace

namespace Shashlik.Identity
{
    public class ShashlikInt32UserManager : ShashlikUserManager<Users, int>
    {
        public ShashlikInt32UserManager(IUserStore<Users> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<Users> passwordHasher,
            IEnumerable<IUserValidator<Users>> userValidators, IEnumerable<IPasswordValidator<Users>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<Users>> logger,
            IUserConfirmation<Users> userConfirmation) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators,
            keyNormalizer, errors, services, logger, userConfirmation)
        {
        }
    }
}