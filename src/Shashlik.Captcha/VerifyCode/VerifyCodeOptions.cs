using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Features.VerifyCode
{
    public class VerifyCodeOptions
    {

        /// <summary>
        /// 验证码过期时间
        /// </summary>
        public int ExpireSecond { get; set; }

        /// <summary>
        /// 可以失败的次数
        /// </summary>
        public int MaxErrorCount { get; set; }
    }
}
