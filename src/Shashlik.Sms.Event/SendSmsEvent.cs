using Shashlik.EventBus;
using System.Collections.Generic;

namespace Shashlik.Sms.Event
{
    public class SendSmsEvent : IEvent
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public List<string> Phones { get; set; }

        /// <summary>
        /// 短信类型
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 短信参数
        /// </summary>
        public List<string> Args { get; set; }
    }
}
