using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.Wx
{

    /// <summary>
    /// JSAPI支付
    /// </summary>
    public class JsApiResult
    {
        /// <summary>
        /// appid
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public string TimeStamp { get; set; }
        /// <summary>
        /// 随机字符串
        /// </summary>
        public string NonceStr { get; set; }
        /// <summary>
        /// 订单详情扩展字符串
        /// </summary>
        public string Package { get; set; }
        /// <summary>
        /// 签名方式
        /// </summary>
        public string SignType { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string PaySign { get; set; }
    }
}
