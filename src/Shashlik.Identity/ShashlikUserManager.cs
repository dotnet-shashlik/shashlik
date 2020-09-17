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
        protected ShashlikIdentityDbContext DbContext { get; }

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
            DbContext = identityDbContext;
        }

        /// <summary>
        /// 根据身份证查找用户,手机号码
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<Users> FindByPhoneNumberAsync(string phoneNumber)
        {
            return await DbContext.Users.SingleOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        /// <summary>
        /// 根据身份证查找用户,身份证号码唯一有效
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        public async Task<Users> FindByIdCardAsync(string idCard)
        {
            return await DbContext.Users.SingleOrDefaultAsync(u => u.IdCard == idCard);
        }
    }
}