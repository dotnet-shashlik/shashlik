using System;
using Microsoft.AspNetCore.Identity;

namespace Shashlik.Identity.Entities
{
    /// <summary>
    /// 角色
    /// </summary>
    public class IdentityRoleBase<TKey> : IdentityRole<TKey> where TKey : IEquatable<TKey>
    {
    }
}