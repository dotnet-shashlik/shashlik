using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Identity.Entities;

namespace Shashlik.Identity
{
    //TODO: 完善方法
    public class ShashlikUserManager : UserManager<Users>
    {
        private readonly ShashlikIdentityDbContext _identityDbContext;
        public ShashlikUserManager(IUserStore<Users> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<Users> passwordHasher,
            IEnumerable<IUserValidator<Users>> userValidators,
            IEnumerable<IPasswordValidator<Users>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<Users>> logger, ShashlikIdentityDbContext identityDbContext)
            : base(store, optionsAccessor, passwordHasher, userValidators,
                passwordValidators, keyNormalizer, errors, services, logger)
        {
            _identityDbContext = identityDbContext;
        }

        public async Task<Users> FindByPhoneNumber(string phoneNumber)
        {
            return await _identityDbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }
    }
}