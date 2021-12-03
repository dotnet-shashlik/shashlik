using Shashlik.Kernel.Attributes;
using Shashlik.Sms.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shashlik.Sms.Aliyun
{
    [AutoOptions("Shashlik.Sms.Aliyun")]
    public class AliyunSmsOptions
    {
        public string EndPoint { get; set; } = "dysmsapi.aliyuncs.com";

        public string AppId { get; set; } = "";

        public string AppKey { get; set; } = "";
    }
}