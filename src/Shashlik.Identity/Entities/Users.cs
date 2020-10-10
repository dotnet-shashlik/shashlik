using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using Shashlik.EfCore;
using Shashlik.Identity.Options;

namespace Shashlik.Identity.Entities
{
    /// <summary>
    /// 用户
    /// </summary>
    public class Users : IdentityUser<int>, IEntity<int>, ISoftDeleted<long>
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

    public class UsersConfig : IEntityTypeConfiguration<Users>
    {
        public UsersConfig(IOptions<IdentityOptionsExtends> options)
        {
            Options = options.Value;
        }

        private IdentityOptionsExtends Options { get; }

        public void Configure(EntityTypeBuilder<Users> builder)
        {
            builder.Property(r => r.IdCard).HasMaxLength(32).IsRequired(Options.RequireIdCard);
            builder.Property(r => r.RealName).HasMaxLength(32).IsRequired(Options.RequireRealName);
            builder.Property(r => r.NickName).HasMaxLength(255).IsRequired(Options.RequireNickName);
            builder.Property(r => r.Avatar).HasMaxLength(255).IsRequired(Options.RequireAvatar);
            builder.Property(r => r.Birthday).IsRequired(Options.RequireBirthday);
            builder.HasIndex(r => r.IdCard).IsUnique(Options.IdCardUnique);
            builder.HasIndex(r => r.PhoneNumber).IsUnique(Options.PhoneNumberUnique);
        }
    }
}