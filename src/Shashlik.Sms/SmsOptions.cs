using System.Collections.Generic;

namespace Guc.Sms
{
    /// <summary>
    /// 短信配置
    /// </summary>
    public class SmsOptions
    {
        /// <summary>
        /// 短信发送限制
        /// </summary>
        public List<_Limit> Limits { get; set; }

        /// <summary>
        /// 短信配置
        /// </summary>
        public List<SmsDomainConfig> DomainConfigs { get; set; }
    }

    /// <summary>
    /// 短信主机配置
    /// </summary>
    public class SmsDomainConfig
    {
        /// <summary>
        /// 排序号
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 短信主机,已定义的有,1:阿里云,2:腾讯
        /// </summary>
        public int Domain { get; set; }

        /// <summary>
        /// 云服务短信 appid
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 云服务短信 appkey
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// 模板列表
        /// </summary>
        public List<_Template> Templates { get; set; }

        /// <summary>
        /// 短信模板
        /// </summary>
        public class _Template
        {
            /// <summary>
            /// 短信类型
            /// </summary>
            public string SmsSubject { get; set; }
            /// <summary>
            /// 云服务短信模板id
            /// </summary>
            public string TemplateId { get; set; }

            /// <summary>
            /// 云服务短信签名
            /// </summary>
            public string SmsSign { get; set; }

        }
    }

    public class _Limit
    {
        public string Subject { get; set; }

        /// <summary>
        /// 每天可以发多少次,空不限制
        /// </summary>
        public int? DayLimitCount { get; set; }
        /// <summary>
        /// 每小时可以发多少次,空不限制
        /// </summary>
        public int? HourLimitCount { get; set; }
        /// <summary>
        /// 每分钟可以发多少次,空不限制
        /// </summary>
        public int? MinuteLimitCount { get; set; }
    }
}
