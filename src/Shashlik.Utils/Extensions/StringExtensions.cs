using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Shashlik.Utils.Extensions
{
    /// <summary>
    /// 字符串扩展类
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 是否为null或者空字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 字符串比较 忽略大小写
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string source, string target)
        {
            if (source == null || target == null)
                return false;
            return source.Equals(target, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 字符串分割为数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="separators"></param>
        /// <returns></returns>
        public static List<T> Split<T>(this string str, params string[] separators) where T : struct
        {
            if (string.IsNullOrWhiteSpace(str))
                return new List<T>();
            return
                str
                    .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    .Select(TypeExtensions.ConvertTo<T>).ToList();
        }

        /// <summary>
        /// 字符串分割为数组,跳过不能转换的异常数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="separators"></param>
        /// <returns></returns>
        public static List<T> SplitSkipError<T>(this string str, params string[] separators) where T : struct
        {
            if (string.IsNullOrWhiteSpace(str))
                return new List<T>();
            var query =
                str
                    .Split(separators, StringSplitOptions.RemoveEmptyEntries);

            List<T> result = new List<T>();
            foreach (var item in query)
            {
                try
                {
                    result.Add(item.ConvertTo<T>());
                }
                catch
                {
                    // ignored
                }
            }

            return result;
        }

        public static bool Contains(this string source, string value, StringComparison stringComparison)
        {
            if (source == null || value == null)
            {
                return false;
            }

            if (value == "")
            {
                return true;
            }

            return (source.IndexOf(value, stringComparison) >= 0);
        }

        /// <summary>
        /// 字符串截取,null时,返回"",不足长度时,返回字符串本身
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SubStringIfTooLong(this string str, int length, string suffix = "...")
        {
            if (string.IsNullOrWhiteSpace(str))
                return "";

            var span = str.AsSpan().Trim();
            if (span.Length <= length)
                return span.ToString();

            StringBuilder sb = new StringBuilder();
            sb.Append(span.Slice(0, length).ToString());
            sb.Append(suffix);
            return sb.ToString();
        }

        /// <summary>
        /// 字符串替换,忽略大小写,使用的是正则,注意正则中的特殊字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <param name="replaceString"></param>
        /// <returns></returns>
        public static string ReplaceIgnoreCase(this string str, string pattern, string replaceString)
        {
            return Regex.Replace(str, pattern, replaceString, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 正则是否匹配
        /// </summary>
        /// <param name="value"></param>
        /// <param name="regexPattern">正则表达式</param>
        /// <returns></returns>
        public static bool IsMatch(this string value, string regexPattern)
        {
            if (value == null)
                return false;
            return Regex.IsMatch(value, regexPattern);
        }

        /// <summary>
        /// 正则是否匹配
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsUrl(this string value)
        {
            if (value == null)
                return false;
            return Regex.IsMatch(value, @"^((https|http)?:\/\/)[^\s]+");
        }


        /// <summary>
        /// 空值转换为null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EmptyToNull(this string value)
        {
            return value.IsNullOrWhiteSpace() ? null : value;
        }

        /// <summary>
        /// 字符串格式化,string.Formart
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static string Format(this string value, params object[] ps)
        {
            return string.Format(value, ps);
        }

        /// <summary>
        /// 字符串脱敏
        /// </summary>
        /// <param name="value"></param>
        /// <param name="beginLength">前面保留几位</param>
        /// <param name="endLength">后面保留几位</param>
        /// <returns></returns>
        public static string ConfidentialData(this string value, int beginLength, int endLength)
        {
            if (value.IsNullOrWhiteSpace())
                return "";

            var span = value.AsSpan();
            StringBuilder sb = new StringBuilder();
            if (value.Length >= beginLength)
                sb.Append(span.Slice(0, beginLength).ToString());
            else
                return value;

            sb.Append("****");

            if (value.Length >= endLength)
                sb.Append(span.Slice(value.Length - endLength).ToString());

            return sb.ToString();
        }

        public static bool StartsWithIgnoreCase(this string value, string starts)
        {
            return value.AsSpan().StartsWith(starts.AsSpan(), StringComparison.OrdinalIgnoreCase);
        }

        public static bool EndsWithIgnoreCase(this string value, string ends)
        {
            return value.AsSpan().EndsWith(ends.AsSpan(), StringComparison.OrdinalIgnoreCase);
        }

        public static string HtmlEncode(this string value)
        {
            return HttpUtility.HtmlEncode(value);
        }

        public static string HtmlDecode(this string value)
        {
            return HttpUtility.HtmlDecode(value);
        }

        public static string UrlEncode(this string value)
        {
            return HttpUtility.UrlEncode(value);
        }

        public static string UrlDecode(this string value)
        {
            return HttpUtility.UrlDecode(value);
        }

        /// <summary>
        /// url参数合并
        /// </summary>
        /// <param name="url"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string UrlArgsCombine(this string url, IEnumerable<KeyValuePair<string, object>> values)
        {
            if (url.IsNullOrWhiteSpace() || values.IsNullOrEmpty())
                return url;

            StringBuilder sb = new StringBuilder();
            sb.Append(url);
            var span = url.AsSpan().Trim();
            if (!span.Contains(new[] {'?'}, StringComparison.OrdinalIgnoreCase))
                sb.Append('?');
            else if (!span.EndsWith(new[] {'&'}, StringComparison.OrdinalIgnoreCase))
                sb.Append('&');

            var count = values.Count();
            for (int i = 0; i < count; i++)
            {
                var item = values.ElementAt(i);
                if (item.Value == null)
                    continue;

                sb.Append(item.Key);
                sb.Append('=');
                sb.Append(item.Value.ToString().UrlEncode());
                sb.Append('&');
            }

            return sb.ToString().TrimEnd('&');
        }

        /// <summary>
        /// 清除字符串中的html标签
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveHtml(this string input)
        {
            var regex = new Regex(@"<.*?>");
            return regex.Replace(input, "");
        }

        /// <summary>
        /// 按文本字符截取字符串，通常用于包含emoji的字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SubStringByTextElements(this string input, int start, int length)
        {
            var stringInfo = new StringInfo(input);
            if (start == 0 && stringInfo.LengthInTextElements <= length)
            {
                return input;
            }

            return stringInfo.SubstringByTextElements(start, length);
        }

        /// <summary>
        /// XML字符串反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T DeserializeXml<T>(this string xml)
        {
            if (string.IsNullOrEmpty(xml)) throw new NotSupportedException("Empty string!!");

            var xmlSerializer = new XmlSerializer(typeof(T));
            var stringReader = new StringReader(xml);
            using var reader = XmlReader.Create(stringReader);
            return (T) xmlSerializer.Deserialize(reader);
        }

        #region Base64位加密解密

        /// <summary>
        /// 将字符串转换成base64格式,使用UTF8字符集
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <param name="encoding"></param>
        /// <param name="urlSafe"></param>
        /// <returns></returns>
        public static string Base64Encode(this string content, Encoding encoding = null, bool urlSafe = false)
        {
            encoding ??= Encoding.UTF8;
            var bytes = encoding.GetBytes(content);
            var str = Convert.ToBase64String(bytes);
            if (!urlSafe)
                return str;
            return str.Replace("=", "")
                    .Replace("+", "-")
                    .Replace("/", "_")
                ;
        }

        /// <summary>
        /// 将base64格式，转换utf8
        /// </summary>
        /// <param name="content">解密内容</param>
        /// <param name="encoding"></param>
        /// <param name="urlSafe"></param>
        /// <returns></returns>
        public static string Base64Decode(this string content, Encoding encoding = null, bool urlSafe = false)
        {
            if (urlSafe)
            {
                content = content.Replace("-", "+").Replace("_", "/");
                var base64 = Encoding.ASCII.GetBytes(content);
                var padding = base64.Length * 3 % 4; //(base64.Length*6 % 8)/2
                if (padding != 0)
                    content = content.PadRight(content.Length + padding, '=');
            }

            encoding ??= Encoding.UTF8;
            var bytes = Convert.FromBase64String(content);
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// 将base64格式，转换utf8
        /// </summary>
        /// <param name="content">解密内容</param>
        /// <returns></returns>
        public static byte[] Base64DecodeToBytes(this string content, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return Convert.FromBase64String(content);
        }

        #endregion

        /// <summary>
        /// 首字母转小写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string LowerFirstCase(this string str)
        {
            if (str.IsNullOrWhiteSpace())
                return str;
            var ch = str[0];
            if (ch >= 65 && ch <= 90)
            {
                var s = new Span<char>(str.ToCharArray()) {[0] = (char) (ch + 32)};
                return s.ToString();
            }

            return str;
        }

        /// <summary>
        /// 首字母转小写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UpperFirstCase(this string str)
        {
            if (str.IsNullOrWhiteSpace())
                return str;
            var ch = str[0];
            if (ch >= 97 && ch <= 122)
            {
                var s = new Span<char>(str.ToCharArray()) {[0] = (char) (ch - 32)};
                return s.ToString();
            }

            return str;
        }
    }
}