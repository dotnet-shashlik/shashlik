using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Mail
{
    public class AliyunDmOptions
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
        /// 控制台创建的发信地址
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 发信人昵称
        /// </summary>
        public string FromAlias { get; set; }
    }
}
