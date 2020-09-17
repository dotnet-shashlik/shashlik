using System.Collections.Generic;
using Shashlik.Kernel.Autowired.Attributes;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// ids4 AspNetCoreIdentity 配置选项
    /// </summary>
    [AutoOptions("Shashlik:Ids4:Identity")]
    public class Ids4IdentityOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 密码登录源数据,允许值:username/phone/email/idCard,也就是可以使用用户名/手机号码/邮件地址作为登录用户名,使用手机号/邮件登录时务必保证其唯一性
        /// </summary>
        public HashSet<string> PasswordSignInSources { get; set; } = new HashSet<string> {Consts.UsernameSource};

        /// <summary>
        /// 手机验证,不存在时是否创建新用户
        /// </summary>
        public bool CreateUserIfNotExistsOnPhoneValidator { get; set; } = false;

        /// <summary>
        /// 邮件验证,不存在时是否创建新用户
        /// </summary>
        public bool CreateUserIfNotExistsOnEmailValidator { get; set; } = false;

        // /// <summary>
        // /// 手机双因子验证,token获取api地址
        // /// </summary>
        // public string GetPhone2FATokenApi { get; set; } = "/identity/phone_2fa_token";
        //
        // /// <summary>
        // /// 邮件双因子验证,token获取api地址
        // /// </summary>
        // public string GetEMail2FATokenApi { get; set; } = "/identity/email_2fa_token";
        //
        // /// <summary>
        // /// 手机短信登录,获取短信验证码api地址
        // /// </summary>
        // public string GetPhoneLoginCodeApi { get; set; } = "/identity/phone_login_code";
        //
        // /// <summary>
        // /// 邮件验证码登录,获取邮件验证码api地址
        // /// </summary>
        // public string GetEMailLoginCodeApi { get; set; } = "/identity/email_login_code";
        //
        // /// <summary>
        // /// 验证码长度
        // /// </summary>
        // public int CodeLength { get; set; } = 6;
    }
}