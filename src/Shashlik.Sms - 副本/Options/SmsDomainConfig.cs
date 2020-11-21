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
        /// 短信主机,已定义的有,aliyun:阿里云,tencent:腾讯云
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// 区域(阿里云有效)
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 云服务短信 appid
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 云服务短信 appKey
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// 主机额外的参数,扩展用
        /// </summary>
        public IDictionary<string, string> Extra { get; set; }

        /// <summary>
        /// 模板列表
        /// </summary>
        public List<SmsTemplates> Templates { get; set; } = new List<SmsTemplates>();
    }
}