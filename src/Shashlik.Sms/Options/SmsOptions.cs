using System.Collections.Generic;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Sms.Options
{
    /// <summary>
    /// 短信配置
    /// </summary>
    [AutoOptions("Shashlik.Sms")]
    public class SmsOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 使用空短信,不真实发送
        /// </summary>
        public bool UseEmptySms { get; set; } = false;

        /// <summary>
        /// 验证码每人每天可以发多少次,0不限制
        /// </summary>
        public int CaptchaDayLimitCount { get; set; } = 20;

        /// <summary>
        /// 验证码每人每小时可以发多少次,0不限制
        /// </summary>
        public int CaptchaHourLimitCount { get; set; } = 10;

        /// <summary>
        /// 验证码每人每分钟可以发多少次,0空不限制
        /// </summary>
        public int CaptchaMinuteLimitCount { get; set; } = 1;

        /// <summary>
        /// 模板列表
        /// </summary>
        public List<SmsTemplates> Templates { get; set; } = new List<SmsTemplates>();
    }
}