using Senparc.Weixin.TenPay.V3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.Wx
{
    /// <summary>
    /// 微信预支付请求结果
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class PrePayResult<TResult>
    {
        // ReSharper disable All 
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 不同支付模式对应的预付单结果
        /// </summary>
        public TResult Result { get; set; }

        /// <summary>
        /// 我方系统交易单号
        /// </summary>
        public string LocalTradeNo { get; set; }

        /// <summary>
        /// 微信统一下单结果
        /// </summary>
        public UnifiedorderResult UnifiedorderResult { get; set; }

#pragma warning restore IDE1006 // 命名样式
        // ReSharper restore All 
    }
}
