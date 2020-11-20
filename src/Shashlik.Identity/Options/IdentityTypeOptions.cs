using System;

namespace Shashlik.Identity.Options
{
    public class IdentityTypeOptions
    {
        public Type? UserType { get; set; }

        public Type? RoleType { get; set; }

        public Type? KeyType { get; set; }
    }
}