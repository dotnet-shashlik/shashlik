using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Wx
{
    /// <summary>
    /// APP支付
    /// </summary>
    public class AppPayResult
    {
        // ReSharper disable All 
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// appid
        /// </summary>
        public string Appid { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string Partnerid { get; set; }

        /// <summary>
        /// 预支付交易会话ID
        /// </summary>
        public string Prepayid { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// 随机字符串
        /// </summary>
        public string Noncestr { get; set; }
        /// <summary>
        /// 订单详情扩展字符串
        /// </summary>
        public string Package { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string Sign { get; set; }

#pragma warning restore IDE1006 // 命名样式
        // ReSharper restore All 
    }
}
