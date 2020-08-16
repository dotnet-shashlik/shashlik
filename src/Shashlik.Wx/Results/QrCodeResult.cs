using Senparc.Weixin.TenPay.V3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Wx
{
    /// <summary>
    /// 二维码支付微信预付单结果
    /// </summary>
    public class QrCodeResult
    {
        // ReSharper disable All 
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 二维码支付地址
        /// </summary>
        public string QrcodeUrl { get; set; }

#pragma warning restore IDE1006 // 命名样式
        // ReSharper restore All 
    }
}
