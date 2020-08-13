using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.AliyunOss
{
    public class AliyunOssOptions
    {
        /// <summary>
        /// 访问密钥ID
        /// </summary>
        public string AccessId { get; set; }
        /// <summary>
        /// 访问密钥
        /// </summary>
        public string AccessKey { get; set; }
        /// <summary>
        /// 访问域名
        /// </summary>
        public string Endpoint { get; set; }
        /// <summary>
        /// 存储空间名称
        /// </summary>
        public string Bucket { get; set; }
        /// <summary>
        /// 主机URL
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// cdn域名
        /// </summary>
        public string CdnHost { get; set; }

        /// <summary>
        /// 上传目录
        /// </summary>
        public string Dir { get; set; }

        /// <summary>
        /// 文件大小限制，MB
        /// </summary>
        public int MaxSize { get; set; }
    }
}
