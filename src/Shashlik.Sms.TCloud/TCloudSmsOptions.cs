using Shashlik.Kernel.Attributes;
using Shashlik.Sms.Options;

namespace Shashlik.Sms.TCloud
{
    [AutoOptions("Shashlik.Sms.TCloud")]
    public class TCloudSmsOptions
    {
        public string EndPoint { get; set; } = "";

        public string Region { get; set; } = "ap-guangzhou";

        public string SecretId { get; set; } = "";

        public string SecretKey { get; set; } = "";

        public string AppId { get; set; } = "";
    }
}