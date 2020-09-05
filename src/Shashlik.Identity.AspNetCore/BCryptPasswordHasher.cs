using Microsoft.AspNetCore.Identity;
using Shashlik.Identity.Entities;
using Shashlik.Utils.Encrypt;

namespace Shashlik.Identity.AspNetCore
{
    public class BCryptPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
    {
        public string HashPassword(TUser user, string password)
        {
            return password.BCrypt();
        }

        public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword,
            string providedPassword)
        {
            //TODO: 什么时候该用PasswordVerificationResult.SuccessRehashNeeded?
            return hashedPassword.BCryptVerify(providedPassword)
                ? PasswordVerificationResult.Success
                : PasswordVerificationResult.Failed;
        }
    }
}