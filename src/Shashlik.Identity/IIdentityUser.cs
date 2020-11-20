using System;

namespace Shashlik.Identity
{
    public interface IIdentityUser
    {
        string IdString { get;}

        string UserName { get; set; }

        string NormalizedUserName { get; set; }

        string Email { get; set; }

        string NormalizedEmail { get; set; }

        bool EmailConfirmed { get; set; }

        string PasswordHash { get; set; }

        string SecurityStamp { get; set; }

        string ConcurrencyStamp { get; set; }

        string PhoneNumber { get; set; }

        bool PhoneNumberConfirmed { get; set; }

        bool TwoFactorEnabled { get; set; }

        DateTimeOffset? LockoutEnd { get; set; }

        bool LockoutEnabled { get; set; }

        int AccessFailedCount { get; set; }

        string? Avatar { get; set; }

        DateTime? Birthday { get; set; }

        string? NickName { get; set; }

        string? IdCard { get; set; }

        string? RealName { get; set; }

        Gender Gender { get; set; }

        long CreateTime { get; set; }

        long LastTime { get; set; }
    }
}