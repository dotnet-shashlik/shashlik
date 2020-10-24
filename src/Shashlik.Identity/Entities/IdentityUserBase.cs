using System;
using Microsoft.AspNetCore.Identity;
using Shashlik.EfCore;

namespace Shashlik.Identity.Entities
{
    /// <summary>
    /// 用户
    /// </summary>
    public class IdentityUserBase<TKey> : IdentityUser<TKey>, ISoftDeleted<long>
        where TKey : IEquatable<TKey>
    {
        private string? _idCard;

        /// <summary>
        /// 头像
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string? NickName { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public string? IdCard
        {
            get => _idCard;
            set => _idCard = value?.ToUpperInvariant();
        }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string? RealName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Gender Gender { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleteTime { get; set; }
    }
}