using System;
using Shashlik.Kernel.Attributes;

// ReSharper disable CheckNamespace

namespace Shashlik.Identity
{
    [ConditionDependsOnMissing(typeof(IIdentityUserRoleDefinition))]
    public class Int32UserRoleDefinition : IIdentityUserRoleDefinition
    {
        public Type UserType => typeof(Users);
        public Type RoleType => typeof(Roles);
    }
}