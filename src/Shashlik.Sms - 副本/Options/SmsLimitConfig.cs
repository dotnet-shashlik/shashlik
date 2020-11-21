namespace Shashlik.Sms.Options
{
      public class SmsLimitConfig
      {
            /// <summary>
            /// 短信类型
            /// </summary>
            public string Subject { get; set; }

            /// <summary>
            /// 每天可以发多少次,空不限制
            /// </summary>
            public int? DayLimitCount { get; set; }

            /// <summary>
            /// 每小时可以发多少次,空不限制
            /// </summary>
            public int? HourLimitCount { get; set; }

            /// <summary>
            /// 每分钟可以发多少次,空不限制
            /// </summary>
            public int? MinuteLimitCount { get; set; }
      }

}