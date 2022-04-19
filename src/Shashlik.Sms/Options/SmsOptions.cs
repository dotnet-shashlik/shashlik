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
        /// 短信提供者名称，具体使用什么短信
        /// </summary>
        public string Provider { get; set; } = EmptySmsProvider.Provider;

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
        /// 模板列表,key: 模板的subject,value:模板配置
        /// </summary>
        public IDictionary<string, SmsTemplates> Templates { get; set; } = new Dictionary<string, SmsTemplates>();
    }
}