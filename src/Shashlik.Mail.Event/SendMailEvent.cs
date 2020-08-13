using Guc.EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.Mail
{
    public class SendMailEvent : IEvent
    {
        public string Address { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }
    }
}
