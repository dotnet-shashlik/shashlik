using Senparc.NeuChar.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Wx
{
    /// <summary>
    /// 将响应进行转发
    /// </summary>
    public class ResponseRedirect : ResponseMessageBase
    {
        public string Host { get; set; }
    }
}
