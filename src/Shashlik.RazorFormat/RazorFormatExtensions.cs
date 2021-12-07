using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Shashlik.Utils.Extensions;

// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UseDeconstruction
// ReSharper disable InvertIf

namespace Shashlik.RazorFormat
{
    public static class RazorFormatExtensions
    {
        // 已注册的格式化器
        private static readonly IDictionary<string, IFormatter> formatters = new Dictionary<string, IFormatter>();

        // 格式化表达式匹配正则
        private static readonly Regex formatExpressionReg = new Regex("([a-zA-Z]{1}\\w{0,15})\\s*\\({1,1}[^\\(^\\)]*\\){1,1}");

        // action
        private static readonly Regex actionReg = new Regex("^[a-zA-Z]{1}\\w{0,15}$");

        /// <summary>
        /// 注册格式化器,不要多线程注册,非线程安全
        /// </summary>
        /// <param name="formater"></param>
        public static void Registry(IFormatter formater)
        {
            if (formater is null)
                throw new ArgumentNullException(nameof(formater));
            if (formater.Action.IsNullOrWhiteSpace()
                || !actionReg.IsMatch(formater.Action))
                throw new ArgumentException("formatter name error!", nameof(formater.Action));
            if (formatters.ContainsKey(formater.Action))
                throw new ArgumentException($"formatter '{formater.Action}' already exists!", nameof(formater.Action));
            formatters.Add(formater.Action, formater);
        }

        /// <summary>
        /// 移除已注册的格式化器
        /// </summary>
        /// <param name="formaterName">格式化器的名称</param>
        /// <param name="formatter">格式化器</param>
        public static bool TryRemove(string formaterName, out IFormatter? formatter)
        {
            formatter = formatters.GetOrDefault(formaterName);
            if (formatter is not null)
            {
                formatters.Remove(formaterName);
                return true;
            }

            return false;
        }

        static RazorFormatExtensions()
        {
            Registry(new SwitchFormatter());
            Registry(new SubIfTooLongFormatter());
            Registry(new SubstrFormatter());
            Registry(new ToggleCaseFormatter());
            Registry(new AddPrefixFormatter());
            Registry(new AddSuffixFormatter());
            Registry(new StringFormatFormatter());
        }

        /// <summary>
        /// razor模板格式化,参数格式:@{arg},参数名区分大小写<para></para>
        /// 1. 支持数据类型本身的格式化format,如@{Money|f2},Money为double类型,|f2表示格式化为2位小数显示<para></para>
        /// 2. 扩展格式化方法,例: value = "1234567890", subIfTooLong(3,...) -> result: "123..."  <para></para>
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="value">值</param>
        /// <param name="model">格式化模型,支持IDictionary/JsonElement/JsonObject</param>
        /// <param name="prefix">格式符前缀,默认@</param>
        /// <param name="throwExceptionWhenPropertyNotExists">属性值不存在时是否抛出异常,false则直接原样字符串输出</param>
        /// <returns>format result value</returns>
        /// <exception cref="MissingMemberException">throwExceptionWhenPropertyNotExists == true 可能抛出成员不存在异常</exception>
        public static string RazorFormat<TModel>(this string value, TModel model, char prefix = '@',
            bool throwExceptionWhenPropertyNotExists = false)
        {
            if (value.IsNullOrWhiteSpace() || model is null)
                return value;

            var reg = new Regex("\\" + prefix + @"\{[^\{^\}]{1,}\}");

            var matches = reg.Matches(value);
            if (matches.Count == 0)
                return value;

            var sb = new StringBuilder();
            var inputSpan = value.AsSpan();

            var lastIndex = 0;
            foreach (Match item in matches)
            {
                if (!item.Success)
                    continue;

                // ignore e.g. @@{Name}
                if (item.Index > 0 && inputSpan[item.Index - 1] == prefix)
                {
                    var newIndex = item.Index + item.Value.Length;
                    sb.Append(inputSpan[lastIndex..newIndex]);
                    lastIndex = newIndex;
                    continue;
                }
                else
                {
                    var newIndex = item.Index;
                    sb.Append(inputSpan[lastIndex..newIndex]);
                    lastIndex = newIndex;
                }

                // proName: 属性名称, formatExp: 格式化表达式
                var (proName, formatExp) = GetFormatContent(item.Value, prefix);

                // 实例属性值
                var proValue = model.GetPropertyValue(proName);
                if (!proValue.exists && !throwExceptionWhenPropertyNotExists)
                    // 属性值不存在, 跳过, 直接原始输出
                    continue;
                if (!proValue.exists && throwExceptionWhenPropertyNotExists)
                    throw new MissingMemberException(typeof(TModel).FullName, proName);

                var objectValue = proValue.value; // object value
                var stringValue = objectValue?.ToString(); // string value, nullable

                if (string.IsNullOrWhiteSpace(formatExp))
                {
                    // 没有格式化表达式, 直接append
                    sb.Append(stringValue);
                    lastIndex += item.Value.Length;
                }
                else
                {
                    var (hasFormatter, formatterStringValue) = GetFormatterStringValue(formatExp, objectValue);
                    if (hasFormatter)
                    {
                        sb.Append(formatterStringValue ?? string.Empty);
                        lastIndex += item.Value.Length;
                    }
                    else
                    {
                        // 没有格式化器,使用标准string format进行格式化
                        var formattedStringValue = string.Format($"{{0:{formatExp}}}", objectValue);
                        sb.Append(formattedStringValue);
                        lastIndex += item.Value.Length;
                    }
                }
            }

            if (lastIndex < inputSpan.Length)
                sb.Append(inputSpan.Slice(lastIndex));

            return sb.ToString();
        }

        private static (string proName, string? formarExpression) GetFormatContent(string matchedValue, char prefix)
        {
            var exp = matchedValue.TrimStart(prefix, '{').TrimEnd('}');

            string proName; // 属性名
            string? formatExp; // 格式化表达式
            var splitIndex = exp.IndexOf('|');
            if (splitIndex != -1)
            {
                formatExp = exp.Substring(splitIndex + 1).Trim();
                proName = exp.Substring(0, splitIndex).Trim();
            }
            else
            {
                proName = exp.Trim();
                formatExp = null;
            }

            return (proName, formatExp);
        }

        /// <summary>
        /// 获取格式化器处理后的值
        /// </summary>
        /// <param name="formatExpression"></param>
        /// <param name="objectValue"></param>
        /// <returns>hasFormatter:有没有匹配的格式化器, formatterStringValue: 经过格式化器格式化后的值</returns>
        private static (bool hasFormatter, string? formatterStringValue) GetFormatterStringValue(
            string formatExpression, object? objectValue)
        {
            var formatMatchList = formatExpressionReg.Matches(formatExpression);
            var lastValue = objectValue;
            var hasFormatter = false;
            foreach (Match formatMatch in formatMatchList)
            {
                if (formatMatch.Success && formatMatch.Groups.Count >= 2)
                {
                    var action = formatMatch.Groups[1].Value;
                    var formater = formatters.GetOrDefault(action);
                    if (formater is not null)
                    {
                        var expression = formatMatch.Value.TrimStart(formater.Action.ToCharArray()).Trim().TrimStart('(').TrimEnd(')').Trim();
                        lastValue = formater.Format(lastValue, expression);
                        hasFormatter = true;
                    }
                }
            }

            return (hasFormatter, lastValue?.ToString());
        }
    }
}