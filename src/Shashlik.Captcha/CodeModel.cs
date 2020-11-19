﻿using System;
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable 8618

namespace Shashlik.Captcha
{
    /// <summary>
    /// 验证码
    /// </summary>
    public class CodeModel
    {
        /// <summary>
        /// 验证码类型
        /// </summary>
        public string Purpose { get; set; }

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
        public DateTimeOffset SendTime { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
