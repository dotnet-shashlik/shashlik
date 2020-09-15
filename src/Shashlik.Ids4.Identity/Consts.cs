using IdentityServer4.Models;

namespace Shashlik.Ids4.Identity
{
    public class Consts
    {
        public const string PhoneGrantType = "phone";
        public const string EMailGrantType = "email";
        public const string Phone2FAGrantType = "phone2fa";
        public const string EMail2FAGrantType = "email2fa";
        public const string PasswordGrantType = GrantType.ResourceOwnerPassword;

        public const string LoginPurpose = "login";
    }
}