using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Guc.Wx
{
    /// <summary>
    /// 微信客服消息
    /// </summary>
    public interface IWxCustomerMsg
    {
        /// <param name="openid"></param>
        /// <param name="accessToken"></param>
        /// <param name="msg"></param>
        string Send(string openid, string msg);
        /// <param name="openList>"></param>
        /// <param name="accessToken"></param>
        /// <param name="msg"></param>
        string Send(IEnumerable<string> openList, string msg);
    }
}
