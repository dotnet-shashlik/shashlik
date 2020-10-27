using System.Collections.Generic;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Ids4.IdentityInt32
{
    /// <summary>
    /// ids4 AspNetCoreIdentity 配置选项
    /// </summary>
    [AutoOptions("Shashlik.Ids4.Identity")]
    public class ShashlikIds4IdentityOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 两阶段登录,一阶段信息的过期时间,单位秒,默认3分钟
        /// </summary>
        public int TwoFactorExpiration { get; set; } = 3 * 60;

        /// <summary>
        /// 密码登录源数据,默认username,允许值:username/phone/email/idCard,也就是可以使用用户名/手机号码/邮件地址作为登录用户名,使用手机号/邮件登录时务必保证其唯一性
        /// </summary>
        public HashSet<string> PasswordSignInSources { get; set; } =
            new HashSet<string> {ShashlikIds4IdentityConsts.UsernameSource};

        /// <summary>
        /// 验证码登录数据源,默认phone/email,允许值:username/phone/email/idCard,也就是可以使用用户名/手机号码/邮件地址作为登录用户名,使用手机号/邮件登录时务必保证其唯一性
        /// </summary>
        public HashSet<string> CaptchaSignInSources { get; set; } =
            new HashSet<string> {ShashlikIds4IdentityConsts.PhoneSource, ShashlikIds4IdentityConsts.EMailSource};
    }
}