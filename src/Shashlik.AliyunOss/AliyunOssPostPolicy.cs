namespace Shashlik.AliyunOss
{
    public class AliyunOssPostPolicy
    {
        /// <summary>
        /// 访问密钥ID
        /// </summary>
        public string AccessId { get; set; }
        /// <summary>
        /// 域名
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 用户表单上传的策略,是经过base64编码过的字符串。
        /// </summary>
        public string Policy { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string Signature { get; set; }
        /// <summary>
        /// 上传策略Policy失效时间
        /// </summary>
        public int Expire { get; set; }
        /// <summary>
        /// 上传目录
        /// </summary>
        public  string Dir { get; set; }
        /// <summary>
        /// 上传后的文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件url
        /// </summary>
        public string Url { get; set; }        
    }
}
