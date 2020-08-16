using System;
using System.Collections.Generic;
using System.Text;
using Aliyun.Acs.Core;

namespace Shashlik.Mail.Aliyun
{
    public class SingleSendMailResponse : AcsResponse
    {
        public string EnvId { get; set; }
    }
}
