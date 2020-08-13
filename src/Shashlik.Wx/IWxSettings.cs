using Guc.Utils.Common;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Helpers;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.TenPay.V3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.Wx
{
    /// <summary>
    /// 微信数据获取接口
    /// </summary>
    public interface IWxSettings
    {
        /// <summary>
        /// 全部微信配置数据
        /// </summary>
        SenparcWeixinSetting Setting { get; }

        /// <summary>
        /// v3支付信息
        /// </summary>
        TenPayV3Info PayV3Info { get; }
        /// <summary>
        /// 获取微信token,公众号通用接口调用的token
        /// </summary>
        string Token { get; }
    }

    class DefaultWxSettings : IWxSettings, Guc.Kernel.Dependency.ISingleton
    {
        public SenparcWeixinSetting Setting => Config.SenparcWeixinSetting;

        private static TenPayV3Info _tenPayV3Info;

        public TenPayV3Info PayV3Info
        {
            get
            {
                if (_tenPayV3Info == null)
                {
                    var key = TenPayHelper.GetRegisterKey(Config.SenparcWeixinSetting);
                    _tenPayV3Info = TenPayV3InfoCollection.Data[key];
                }
                return _tenPayV3Info;
            }
        }

        public string Token
        {
            get
            {
                return AccessTokenContainer.GetAccessTokenAsync(Setting.WeixinAppId).GetAwaiter().GetResult();
            }
        }
    }
}
