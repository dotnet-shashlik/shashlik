using System;
using Microsoft.AspNetCore.Identity;
using Shashlik.EfCore;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CheckNamespace

namespace Shashlik.Identity
{
    /// <summary>
    /// 用户
    /// </summary>
    public class Users : IdentityUser<int>, IIdentityUser, ISoftDeleted<long>
    {
        public string IdString => Id.ToString();
        public string? Avatar { get; set; }
        public DateTime? Birthday { get; set; }
        public string? NickName { get; set; }
        public string? IdCard { get; set; }
        public string? RealName { get; set; }
        public int Gender { get; set; }
        public long CreateTime { get; set; }
        public long LastTime { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeleteTime { get; set; }
    }
}