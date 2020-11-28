using System.Text.RegularExpressions;
using Shashlik.Utils;

namespace Shashlik.Sms.Inner
{
    /// <summary>
    /// 短信服务扩展类
    /// </summary>
    internal static class InnerExtensions
    {
        internal static bool IsPhone(this string phone)
        {
            return !string.IsNullOrWhiteSpace(phone)
                   && Regex.IsMatch(phone, @"^[1][0-9]{10}$");
        }
    }
}