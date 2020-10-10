using Microsoft.AspNetCore.Identity;
using Shashlik.Kernel.Autowired.Attributes;

namespace Shashlik.Identity.Options
{
    /// <summary>
    /// 对IdentityOptions配置的扩展配置,<see cref="IdentityOptions.User"/>
    /// </summary>
    [AutoOptions("Shashlik.Identity.IdentityOptions.User")]
    public class IdentityOptionsExtends
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
    }
}