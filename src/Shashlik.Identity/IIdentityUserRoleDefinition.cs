using System;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Identity
{
    /// <summary>
    /// identity,User/Role类型定义
    /// </summary>
    public interface IIdentityUserRoleDefinition : ISingleton
    {
        Type UserType { get; }

        Type RoleType { get; }
    }
}