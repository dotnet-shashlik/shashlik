using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Shashlik.Utils.Extensions;
using TencentCloud.Ecm.V20190719.Models;

namespace TencentFaceId.Sdk
{
    /// <summary>
    /// 腾讯云对象存储cos配置
    /// </summary>
    public class TencentFaceIdOptions
    {
        /// <summary>
        /// appid
        /// </summary>
        public string SecretId { get; set; }

        /// <summary>
        /// appkey
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }

        public string RuleId { get; set; }
    }
}
