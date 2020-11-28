#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

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
        public static bool IsNullOrWhiteSpace(this string? str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 字符串比较 忽略大小写
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="stringComparison"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string? source, string? target,
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (source is null || target is null) return false;
            return source.Equals(target, stringComparison);
        }

        /// <summary>
        /// 字符串分割为数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="separators"></param>
        /// <returns></returns>
        public static List<T> Split<T>(this string? str, params string[] separators) where T : struct
        {
            if (string.IsNullOrWhiteSpace(str))
                return new List<T>();
            return
                str
                    .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    .Select(TypeExtensions.ParseTo<T>).ToList();
        }

        /// <summary>
        /// 字符串分割为数组,跳过不能转换的异常数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="separators"></param>
        /// <returns></returns>
        public static List<T> SplitSkipError<T>(this string? str, params string[] separators) where T : struct
        {
            if (string.IsNullOrWhiteSpace(str))
                return new List<T>();
            var query = str.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            List<T> result = new List<T>();
            foreach (var item in query)
            {
                try
                {
                    result.Add(item.ParseTo<T>());
                }
                catch
                {
                    // ignored
                }
            }

            return result;
        }

        public static bool Contains(this string? source, string? value, StringComparison stringComparison)
        {
            if (source is null || value is null)
                return false;

            if (value == string.Empty)
                return true;

            return (source.IndexOf(value, stringComparison) >= 0);
        }

        /// <summary>
        /// 字符串截取,null时,返回"",不足长度时,返回字符串本身
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length">保留长度</param>
        /// <param name="suffix">后缀字符</param>
        /// <returns></returns>
        public static string SubStringIfTooLong(this string? str, int length, string suffix = "...")
        {
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

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
        /// <param name="pattern">正则</param>
        /// <param name="replaceString">替换字符串</param>
        /// <returns></returns>
        public static string ReplaceIgnoreCase(this string str, string pattern, string replaceString)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            if (replaceString == null) throw new ArgumentNullException(nameof(replaceString));
            return Regex.Replace(str, pattern, replaceString, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 正则是否匹配
        /// </summary>
        /// <param name="value"></param>
        /// <param name="regexPattern">正则表达式</param>
        /// <returns></returns>
        public static bool IsMatch(this string? value, string regexPattern)
        {
            if (value is null) return false;
            if (regexPattern == null) throw new ArgumentNullException(nameof(regexPattern));
            return Regex.IsMatch(value, regexPattern);
        }

        /// <summary>
        /// 空字符串转换为null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string? EmptyToNull(this string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        /// <summary>
        /// 字符串格式化,string.Format
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static string Format(this string value, params object[] ps)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return string.Format(value, ps);
        }

        /// <summary>
        /// 字符串脱敏
        /// </summary>
        /// <param name="value"></param>
        /// <param name="beginLength">前面保留几位</param>
        /// <param name="endLength">后面保留几位</param>
        /// <param name="mixStr">混淆字符</param>
        /// <returns></returns>
        public static string ConfidentialData(this string? value, int beginLength, int endLength, string mixStr = "****")
        {
            if (mixStr == null) throw new ArgumentNullException(nameof(mixStr));
            if (beginLength < 0)
                throw new ArgumentOutOfRangeException(nameof(beginLength));
            if (endLength < 0)
                throw new ArgumentOutOfRangeException(nameof(endLength));

            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var span = value.AsSpan();
            var sb = new StringBuilder();
            if (value.Length >= beginLength)
                sb.Append(span.Slice(0, beginLength).ToString());
            else
                sb.Append(value);

            sb.Append(mixStr);

            if (value.Length >= endLength)
                sb.Append(span.Slice(value.Length - endLength).ToString());
            else
                sb.Append(value);

            return sb.ToString();
        }

        /// <summary>
        /// is starts with "<paramref name="starts"/>" with ignore case
        /// </summary>
        /// <param name="value"></param>
        /// <param name="starts"></param>
        /// <returns></returns>
        public static bool StartsWithIgnoreCase(this string? value, string? starts)
        {
            if (value is null) return false;
            if (starts is null) return false;
            return value.AsSpan().StartsWith(starts.AsSpan(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// is ends with "<paramref name="ends"/>" with ignore case
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ends"></param>
        /// <returns></returns>
        public static bool EndsWithIgnoreCase(this string? value, string? ends)
        {
            if (value is null) return false;
            if (ends is null) return false;
            return value.AsSpan().EndsWith(ends.AsSpan(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// html encode
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string value)
        {
            return HttpUtility.HtmlEncode(value);
        }

        /// <summary>
        /// html decode
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string value)
        {
            return HttpUtility.HtmlDecode(value);
        }

        /// <summary>
        /// url encode
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlEncode(this string value)
        {
            return HttpUtility.UrlEncode(value);
        }

        /// <summary>
        /// url decode
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlDecode(this string value)
        {
            return HttpUtility.UrlDecode(value);
        }

        /// <summary>
        /// url参数合并
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="values">key/value键值对参数</param>
        /// <returns></returns>
        public static string? UrlArgsCombine(this string? url, IEnumerable<KeyValuePair<string, object>>? values)
        {
            if (values is null) return url;
            var keyValuePairs = values.ToList();
            if (url.IsNullOrWhiteSpace() || keyValuePairs.IsNullOrEmpty())
                return url;

            var sb = new StringBuilder();
            sb.Append(url);
            var span = url.AsSpan().Trim();
            if (!span.Contains(new[] {'?'}, StringComparison.OrdinalIgnoreCase))
                sb.Append('?');
            else if (!span.EndsWith(new[] {'&'}, StringComparison.OrdinalIgnoreCase))
                sb.Append('&');

            var count = keyValuePairs.Count();
            for (var i = 0; i < count; i++)
            {
                var item = keyValuePairs.ElementAt(i);
                if (item.Value is null)
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

        #region Base64编码/解码

        /// <summary>
        /// 将字符串转换成base64格式,使用UTF8字符集
        /// </summary>
        /// <param name="content">原始内容</param>
        /// <param name="encoding">编码方式,默认utf8</param>
        /// <param name="urlSafe">是否使用url安全方式编码,参考java的encodeBase64URLSafe</param>
        /// <returns></returns>
        public static string Base64Encode(this string content, Encoding? encoding = null, bool urlSafe = false)
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
        /// 解码base64文本内容
        /// </summary>
        /// <param name="content">已编码内容</param>
        /// <param name="encoding">编码方式,默认utf8</param>
        /// <param name="urlSafe">是否使用url安全方式编码,参考java的encodeBase64URLSafe</param>
        /// <returns></returns>
        public static string Base64Decode(this string content, Encoding? encoding = null, bool urlSafe = false)
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
        public static byte[] Base64DecodeToBytes(this string content)
        {
            return Convert.FromBase64String(content);
        }

        #endregion

        /// <summary>
        /// 首字母转小写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string? LowerFirstCase(this string? str)
        {
            if (str.IsNullOrWhiteSpace())
                return str;
            var ch = str![0];
            if (ch >= 65 && ch <= 90)
            {
                var s = new Span<char>(str.ToCharArray()) {[0] = (char) (ch + 32)};
                return s.ToString();
            }

            return str;
        }

        /// <summary>
        /// 首字母转大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string? UpperFirstCase(this string? str)
        {
            if (str.IsNullOrWhiteSpace())
                return str;
            var ch = str![0];
            if (ch >= 97 && ch <= 122)
            {
                var s = new Span<char>(str.ToCharArray()) {[0] = (char) (ch - 32)};
                return s.ToString();
            }

            return str;
        }
    }
}