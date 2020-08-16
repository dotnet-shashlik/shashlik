using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Shashlik.Utils.Extensions;

namespace TencentCos.Sdk
{
    /// <summary>
    /// 腾讯云对象存储cos配置
    /// </summary>
    public class TencentCosOptions
    {
        /// <summary>
        /// 存储桶域名
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// appid
        /// </summary>
        public string SecretId { get; set; }

        /// <summary>
        /// appkey
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 存储桶的名称
        /// </summary>
        public string BucketName { get; set; }

        /// <summary>
        /// 存储桶地区
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 临时密钥获取时,账户名
        /// </summary>
        public string TempKeyGetterAccountName { get; set; }

        /// <summary>
        /// 临时密钥获取时,授权策略
        /// </summary>
        public TencentPolicy TempKeyGetterPolicy { get; set; }

        /// <summary>
        /// 临时密钥获取时,密钥获取时长毫秒,默认1800
        /// </summary>
        public long? TempKeyGetterDurationSeconds { get; set; }

        public string BuildPolicy(string basePathAfterAllowPath)
        {
            if (TempKeyGetterPolicy == null)
                throw new ArgumentException($"TempKeyGetterPolicy参数为空");
            if (Region.IsNullOrWhiteSpace())
                throw new ArgumentException($"Region参数为空");
            if (TempKeyGetterPolicy.Action.IsNullOrEmpty())
                throw new ArgumentException($"TempKeyGetterPolicy.Action参数为空");
            if (TempKeyGetterPolicy.AllowPath.IsNullOrWhiteSpace())
                throw new ArgumentException($"TempKeyGetterPolicy.AllowPath参数为空");
            if (BucketName.IsNullOrWhiteSpace())
                throw new ArgumentException($"BucketName参数为空");
            if (Region.IsNullOrWhiteSpace())
                throw new ArgumentException($"Region参数为空");
            if (basePathAfterAllowPath.IsNullOrWhiteSpace())
                throw new ArgumentException($"basePathAfterAllowPath参数为空");

            var arr = BucketName?.Split('-');
            if (arr?.Length != 2)
                throw new ArgumentException("BucketName参数错误");
            var userid = arr[1];

            var obj = new
            {
                version = "2.0",
                statement = new[]
                {
                    new
                    {
                        action=TempKeyGetterPolicy.Action,
                        effect=TempKeyGetterPolicy.Effect.ToString(),
                        resource=$"qcs::cos:{Region}:uid/{userid}:{BucketName}/{ TempKeyGetterPolicy.AllowPath.Trim('/') + "/" + basePathAfterAllowPath.Trim('/')}/*"
                    }
                }
            };

            return HttpUtility.UrlEncode(JsonConvert.SerializeObject(obj));
        }
    }
}
