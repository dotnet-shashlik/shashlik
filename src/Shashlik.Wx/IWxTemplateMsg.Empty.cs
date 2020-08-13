using Senparc.Weixin.MP.AdvancedAPIs;
using System;
using System.Collections.Generic;
using System.Text;
using Senparc.Weixin;
using Senparc.Weixin.MP.Containers;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Guc.Utils.Extensions;

namespace Guc.Wx
{
    /// <summary>
    /// 空的微信模板消息
    /// </summary>
    public class EmptyWxTemplateMsg : IWxTemplateMsg
    {
        public void SendWithCode(string openid, string templateCode, object data)
        {
        }

        public void SendWithCodeByDicData(string openid, string templateCode, IDictionary<string, string> data)
        {
        }
    }
}