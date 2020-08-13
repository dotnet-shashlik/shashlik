using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.AliVideo
{
    public class AliVideoOptions
    {
        /// <summary>
        /// 访问密钥ID
        /// </summary>
        public string AccessId { get; set; }
        /// <summary>
        /// 访问密钥
        /// </summary>
        public string AccessKey { get; set; }

        public string AccountId { get; set; }

        public string VodCallbackPrivateKey { get; set; }

        public string RegionId { get; set; }
    }
}
