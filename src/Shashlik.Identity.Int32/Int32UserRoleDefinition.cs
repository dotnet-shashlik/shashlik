using Shashlik.Kernel.Attributes;

// ReSharper disable CheckNamespace

namespace Shashlik.Identity
{
    [ConditionDependsOnMissing(typeof(IIdentityTypeConfigure))]
    public class Int32UserRoleDefinition : IIdentityTypeConfigure<Users, Roles, int>
    {
    }
}