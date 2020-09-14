using IdentityServer4.Models;

namespace Shashlik.Ids4.Identity
{
    public class Ids4IdentityGrantTypes
    {
        public const string Phone = "phone";
        public const string EMail = "email";
        public const string Password = GrantType.ResourceOwnerPassword;
    }
}