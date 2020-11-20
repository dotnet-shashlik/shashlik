using Shashlik.Kernel.Attributes;

// ReSharper disable CheckNamespace

namespace Shashlik.Identity
{
    [ConditionDependsOnMissing(typeof(IIdentityTypeConfigure))]
    public class Int32IdentityTypeConfigure : IIdentityTypeConfigure<Users, Roles, int>
    {
    }
}