using System.Collections.Generic;

namespace Shashlik.Sms.Options
{
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
        /// 区域
        /// </summary>
        public string Region { get; set; }

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
        public List<SmsTemplates> Templates { get; set; }
    }
}