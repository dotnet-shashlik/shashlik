using System;
using System.Collections.Generic;
using Shashlik.Identity.Entities;
using Shashlik.Kernel.Autowire.Attributes;

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
        /// 手机验证,不存在时是否创建新用户
        /// </summary>
        public bool CreateUserIfNotExistsOnPhoneValidator { get; set; } = false;

        /// <summary>
        /// 邮件验证,不存在时是否创建新用户
        /// </summary>
        public bool CreateUserIfNotExistsOnEMailValidator { get; set; } = false;

        /// <summary>
        /// 手机双因子验证,token获取api地址
        /// </summary>
        public string GetPhoneTwoFactorApi { get; set; } = "/identity/phone_token";

        /// <summary>
        /// 邮件双因子验证,token获取api地址
        /// </summary>
        public string GetEMailTwoFactorApi { get; set; } = "/identity/email_token";

        /// <summary>
        /// 用户创建时,可以自定义新用户属性
        /// </summary>
        public Action<Users> UserCreating { get; set; }

        /// <summary>
        /// 用户创建完成后
        /// </summary>
        public Action<Users> UserCreated { get; set; }
    }
}