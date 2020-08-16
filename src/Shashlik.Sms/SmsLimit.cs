using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Sms
{
    public class SmsLimit
    {
        public int Day { get; set; }

        public List<Record> Records { get; set; }

        public class Record
        {
            public int Hour { get; set; }

            public int Minute { get; set; }
        }
    }
}
