using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.Wx
{
    /// <summary>
    /// wx api参数
    /// </summary>
    public class WxApiOptions
    {
        /// <summary>
        /// 获取jssdk 配置对象
        /// </summary>
        public string WxJsSdk { get; set; } = "/wxjssdk";

        ///// <summary>
        ///// 支付回调
        ///// </summary>
        //public string WxPayNotify { get; set; } = "/wxpay/notify";

        ///// <summary>
        ///// snsapi_base 微信登录
        ///// </summary>
        //public string WxOauthSnsapiBase { get; set; } = "/wxoauth/snsapi_base";

        ///// <summary>
        ///// snsapi_base 微信登录 回调地址
        ///// </summary>
        //public string WxOauthSnsapiBaseCallback { get; set; } = "/wxoauth/snsapi_base/callback";

        ///// <summary>
        ///// snsapi_userinfo 微信登录
        ///// </summary>
        //public string WxOauthSnsapiUserinfo { get; set; } = "/wxoauth/snsapi_userinfo";

        ///// <summary>
        ///// snsapi_userinfo 微信登录回调
        ///// </summary>
        //public string WxOauthSnsapiUserinfoCallback { get; set; } = "/wxoauth/snsapi_userinfo/callback";

        /// <summary>
        /// 接受微信服务器的消息/事件推送
        /// </summary>
        public string WxServerResponse { get; set; } = "/wxserver/response";

    }
}
