using System;

namespace Shashlik.Captcha.DistributedCache
{
    /// <summary>
    /// 验证码
    /// </summary>
    public class CaptchaModel
    {
        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 已错误次数
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// 最大错误次数
        /// </summary>
        public int MaxErrorCount { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTimeOffset ExpireAt { get; set; }
    }
}