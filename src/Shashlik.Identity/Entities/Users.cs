using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shashlik.EfCore;

namespace Shashlik.Identity.Entities
{
    /// <summary>
    /// 用户
    /// </summary>
    public class Users : IdentityUser<int>, IEntity<int>, ISoftDeleted<long>
    {
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
        public string? IdCard { get; set; }

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

    public class UsersConfig : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            builder.Property(r => r.IdCard).HasMaxLength(32);
            builder.Property(r => r.RealName).HasMaxLength(32);
            builder.Property(r => r.NickName).HasMaxLength(255);
            builder.Property(r => r.Avatar).HasMaxLength(255);
        }
    }
}