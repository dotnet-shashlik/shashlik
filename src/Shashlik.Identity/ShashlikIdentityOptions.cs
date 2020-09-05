using Shashlik.Kernel.Autowire.Attributes;

namespace Shashlik.Identity
{
    [AutoOptions("Shashlik:Identity")]
    public partial class ShashlikIdentityOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 是否自动迁移
        /// </summary>
        public bool AutoMigration { get; set; }

        /// <summary>
        /// 使用BCrypt作为密码hash算法
        /// </summary>
        public bool UseBCryptPasswordHasher { get; set; }

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
    }
}