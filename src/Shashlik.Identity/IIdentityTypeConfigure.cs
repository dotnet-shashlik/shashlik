using System;
using Microsoft.AspNetCore.Identity;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Identity
{
    public interface IIdentityTypeConfigure
    {
    }

    /// <summary>
    /// identity user/role/key类型定义
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    [Transient(RequireRegistryInheritedChain = true)]
    public interface IIdentityTypeConfigure<TUser, TRole, TKey>
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TKey>, IIdentityUser
        where TRole : IdentityRole<TKey>, IIdentityRole
    {
    }
}