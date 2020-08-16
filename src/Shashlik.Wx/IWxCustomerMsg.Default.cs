using Senparc.Weixin.MP.AdvancedAPIs;
using System;
using System.Collections.Generic;
using System.Text;
using Senparc.Weixin;
using Senparc.Weixin.MP.Containers;
using System.Threading.Tasks;
using System.Linq;
using Shashlik.Utils.Extensions;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Senparc.Weixin.MP.AdvancedAPIs.CustomService;

namespace Shashlik.Wx
{
    /// <summary>
    /// 微信模板消息
    /// </summary>
    class DefaultWxCustomerMsg : IWxCustomerMsg, Shashlik.Kernel.Dependency.ITransient
    {
        //CustomApi
        public DefaultWxCustomerMsg(
            IWxSettings wxSettings
            )
        {
            this.wxSettings = wxSettings;
        }

        IWxSettings wxSettings { get; }

        public string Send(string openid, string msg)
        {
            var token = wxSettings.Token;
            var result = CustomApi.SendTextAsync(token,
                  openid, msg);
            if (result.Result.errcode == 0)
            {
                return "发送成功";
            }
            else
                return "发送失败";
        }

        public string Send(IEnumerable<string> openList, string msg)
        {
            //TODO:优化 并行,返回结果            
            var token = wxSettings.Token;
            var list = openList.ToList();
            foreach (var item in list)
            {
                var result = CustomApi.SendTextAsync(token,
                  item, msg);
                if (result.Result.errcode != 0)
                {
                    return "发送失败！";
                }
            }
            return "发送完成！";
        }

    }
}