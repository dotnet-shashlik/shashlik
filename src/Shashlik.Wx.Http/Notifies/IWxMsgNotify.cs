using Senparc.NeuChar;
using Senparc.NeuChar.Entities;
using Senparc.Weixin.MP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Guc.Wx.Notifies
{
    public interface IWxMsgNotify<TM> : Guc.Kernel.Dependency.ITransient
        where TM : class, IRequestMessageBase
    {
        /// <summary>
        /// 处理推送的消息,如果不需要返回任何信息到微信,直接返回null
        /// </summary>
        /// <param name="inputMsg">输入的消息模型,可根据具体的类型转换,具体类型在此命名空间下<see cref="Senparc.Weixin.MP.Entities"/>,也可以使用dynamic</param>
        /// <returns></returns>
        ResponseMessageBase Handle(TM inputMsg);
    }
}
