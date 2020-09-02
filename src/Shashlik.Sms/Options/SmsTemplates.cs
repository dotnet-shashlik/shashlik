using System.Collections.Generic;
using Shashlik.Kernel.Autowire.Attributes;

namespace Shashlik.Sms.Options
{
    /// <summary>
    /// 短信模板
    /// </summary>
    public class SmsTemplates
    {
        /// <summary>
        /// 短信类型
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 云服务短信模板id
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 云服务短信签名
        /// </summary>
        public string Sign { get; set; }
    }
}