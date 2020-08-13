using Senparc.Weixin.TenPay.V3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.Wx
{
    /// <summary>
    /// 微信H5支付预付单结果
    /// </summary>
    public class H5PayResult
    {
        // ReSharper disable All 
#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// H5支付跳转地址
        /// </summary>
        public string MwebUrl { get; set; }

#pragma warning restore IDE1006 // 命名样式
        // ReSharper restore All 
    }
}
