using System.Collections.Generic;
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
        /// 启用的验证器,phone/email/password
        /// </summary>
        public HashSet<string> AllowValidator { get; set; } = new HashSet<string>();
    }
}