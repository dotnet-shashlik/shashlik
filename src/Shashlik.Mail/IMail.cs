using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Mail
{
    public interface IMail
    {
        /// <summary>
        /// 发送普通HTML邮件
        /// </summary>
        /// <param name="address"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        void Send(string address, string subject, string content);
    }
}
