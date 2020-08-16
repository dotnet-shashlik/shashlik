using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shashlik.Wx
{
    /// <summary>
    /// 微信模板消息
    /// </summary>
    public interface IWxTemplateMsg
    {
        /// <summary>
        /// 格式化的模板消息发送
        /// </summary>
        /// <param name="openid">微信openid</param>
        /// <param name="templateCode">模板code,不区分大小写</param>
        /// <param name="data">模板数据</param>
        void SendWithCode(string openid, string templateCode, object data);
    }
}
