using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Shashlik.Utils.Common
{
    public class TimerHelper
    {
        /// <summary>
        /// 在指定时间过后执行指定的表达式
        /// </summary>
        /// <param name="interval">时间（以毫秒为单位）</param>
        /// <param name="action">要执行的表达式</param>
        /// <return>返回timer对象</return>
        public static void SetTimeout(Action action, TimeSpan expire)
        {
            Task.Run(() =>
            {
                Task.Delay((int)expire.TotalMilliseconds).Wait();
                action();
            });
        }
    }
}
