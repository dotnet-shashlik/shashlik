using System;

// ReSharper disable CheckNamespace

namespace Shashlik.Identity
{
    public class Int32UserRoleDefinition : IIdentityUserRoleDefinition
    {
        public Type UserType => typeof(Users);
        public Type RoleType => typeof(Roles);
    }
}