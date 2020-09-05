using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Identity.Entities;

namespace Shashlik.Identity
{
    public class UserManager : UserManager<Users>
    {
        public UserManager(IUserStore<Users> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<Users> passwordHasher,
            IEnumerable<IUserValidator<Users>> userValidators,
            IEnumerable<IPasswordValidator<Users>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<Users>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators,
                passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }
}