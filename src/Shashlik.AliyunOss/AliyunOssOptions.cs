using System.Collections.Generic;
using Shashlik.Kernel.Attributes;

// ReSharper disable IdentifierTypo
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Shashlik.AliyunOss
{
    [AutoOptions("Shashlik.AliyunOss")]
    public class AliyunOssOptions
    {
        public bool Enable { get; set; }

        /// <summary>
        /// 支持注册多个存储桶
        /// </summary>
        public List<_Buckets> Buckets { get; set; }

        public class _Buckets
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
            /// 是否做为默认bucket
            /// </summary>
            public bool IsDefault { get; set; }

            /// <summary>
            /// 主机URL
            /// </summary>
            public string Host { get; set; }
            
            /// <summary>
            /// cdn域名
            /// </summary>
            public string CdnHost { get; set; }
        }
    }
}