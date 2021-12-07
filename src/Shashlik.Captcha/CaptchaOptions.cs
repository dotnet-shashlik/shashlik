using Shashlik.Kernel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shashlik.Captcha
{
    [AutoOptions("Shashlik.Captcha")]
    public class CaptchaOptions
    {
        public bool Enable { get; set; }
    }
}
