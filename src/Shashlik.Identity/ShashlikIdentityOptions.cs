using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Shashlik.Identity.DataProtection;
using Shashlik.Kernel.Autowired.Attributes;

namespace Shashlik.Identity
{
    [AutoOptions("Shashlik.Identity")]
    public partial class ShashlikIdentityOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// 是否自动迁移
        /// </summary>
        public bool AutoMigration { get; set; }

        /// <summary>
        /// 迁移的程序集名称,如果更改 UserProperty或者identity默认项的配置,需要自行生成迁移,可以传入此参数使用传入的程序集进行自动迁移
        /// </summary>
        public string? MigrationAssembly { get; set; }

        /// <summary>
        /// 原生的identity options配置
        /// </summary>
        public IdentityOptions IdentityOptions { get; set; }

        /// <summary>
        /// 数据保护token配置项
        /// </summary>
        public DataProtectionTokenProviderOptions DataProtectionTokenProviderOptions { get; set; }

        /// <summary>
        /// users表属性配置
        /// </summary>
        public ShashlikIdentityUserPropertyOptions UserProperty { get; set; } =
            new ShashlikIdentityUserPropertyOptions();
    }

    public class ShashlikIdentityUserPropertyOptions
    {
        /// <summary>
        /// 头像是否必填
        /// </summary>
        public bool RequireAvatar { get; set; }

        /// <summary>
        /// 昵称是否必填
        /// </summary>
        public bool RequireNickName { get; set; }

        /// <summary>
        /// 真实姓名是否必填
        /// </summary>
        public bool RequireRealName { get; set; }

        /// <summary>
        /// 身份证号码是否必填
        /// </summary>
        public bool RequireIdCard { get; set; }

        /// <summary>
        /// 生日是否必填
        /// </summary>
        public bool RequireBirthday { get; set; }

        /// <summary>
        /// 身份证号码是否唯一,默认true
        /// </summary>
        public bool IdCardUnique { get; set; } = true;

        /// <summary>
        /// 手机号码是否唯一,默认true
        /// </summary>
        public bool PhoneNumberUnique { get; set; } = true;

        /// <summary>
        /// 邮件地址是否唯一,默认true
        /// </summary>
        public bool EmailUnique { get; set; } = true;

        //TODO: 和IdentityOptions.UserOptions冲突
    }
}