using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Shashlik.Utils;
using Shashlik.Utils.Extensions;

namespace Shashlik.Sms.Inner
{
    /// <summary>
    /// 短信服务扩展类
    /// </summary>
    internal static class Extensions
    {
        internal static bool IsPhone(this string phone)
        {
            return !string.IsNullOrWhiteSpace(phone)
                   && Regex.IsMatch(phone, Consts.Regexs.MobilePhoneNumber);
        }
    }
}