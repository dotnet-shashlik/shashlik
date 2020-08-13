using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.Features.VerifyCode
{
    /// <summary>
    /// 验证码
    /// </summary>
    public class Codes
    {
        public long Id { get; set; }

        /// <summary>
        /// 验证码类型
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 发送目标
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 已验证错误的次数,最大3次
        /// </summary>
        public byte ErrorCount { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public long SendTime { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public long ExpiresAt { get; set; }
    }
}
