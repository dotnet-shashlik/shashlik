namespace Shashlik.Sms.Options
{
    /// <summary>
    /// 短信模板
    /// </summary>
    public class SmsTemplates
    {
        /// <summary>
        /// 短信类型,应用自定义
        /// </summary>
        public string Subject { get; set; } = "";

        /// <summary>
        /// 云服务短信模板id
        /// </summary>
        public string TemplateId { get; set; } = "";

        /// <summary>
        /// 云服务短信签名
        /// </summary>
        public string Sign { get; set; } = "";

        /// <summary>
        /// 模板参数,如果是阿里云/7牛等使用占位符的模板,需要按照模板配置的变量名称进行配置,腾讯云不需要<para></para>
        /// 例: 短信模板为"您的验证码为${code},有效期为${expire}分钟!"<para></para>
        /// 其中变量顺序为code/expire,那么Params参数配置为 ["code","expire"]<para></para>
        /// !!!一定注意变量顺序和变量名称不能错<para></para>
        /// </summary>
        public string[]? Params { get; set; }
    }
}