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
        private static readonly Regex formatExpressionReg = new Regex("^([a-zA-Z]{1}\\w{0,15})\\s*\\([\\s\\S]*\\)$");

        // action
        private static readonly Regex actionReg = new Regex("^[a-zA-Z]{1}\\w{0,15}$");

        /// <summary>
        /// 注册格式化器,不要多线程注册,非线程安全,默认已注册switch格式化器,注册时不会验证action的正确性!!!
        /// </summary>
        /// <param name="formater"></param>
        public static void Registry(IFormatter formater)
        {
            if (formater is null)
                throw new ArgumentNullException(nameof(formater));
            if (formater.Action.IsNullOrWhiteSpace()
                || !actionReg.IsMatch(formater.Action))
                throw new ArgumentException("action name error!", nameof(formater.Action));
            if (formatters.ContainsKey(formater.Action))
                throw new ArgumentException($"action '{formater.Action}' already exists!", nameof(formater.Action));
            formatters.Add(formater.Action, formater);
        }

        static RazorFormatExtensions()
        {
            var formater = new SwitchFormatter();
            Registry(formater);
        }

        /// <summary>
        /// razor模板格式化,参数格式:@{arg},参数名区分大小写,支持数据类型本身的格式化format,如@{Money|f2},Money为double类型,|f2表示格式化为2位小数显示,支持递归格式化
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="value">值</param>
        /// <param name="model">格式化模型,支持IDictionary/JsonElement/JsonObject</param>
        /// <param name="prefix">格式符前缀,默认@</param>
        /// <returns></returns>
        public static string RazorFormat<TModel>(this string value, TModel model, char prefix = '@')
        {
            if (value.IsNullOrWhiteSpace() || model is null)
                return value;

            var reg = new Regex("\\" + prefix + @"\{[^\{\}]{1,}\}");

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

                var exp = item.Value.TrimStart(prefix, '{').TrimEnd('}');

                string proName; // 属性名
                string formatExp = null; // 格式化表达式
                var splitIndex = exp.IndexOf('|');
                if (splitIndex != -1)
                {
                    formatExp = exp.Substring(splitIndex + 1).Trim();
                    proName = exp.Substring(0, splitIndex).Trim();
                }
                else
                    proName = exp.Trim();

                var proValue = model.GetPropertyValue(proName);
                if (!proValue.exists)
                    continue;
                var v = proValue.value;

                if (formatExp.IsNullOrWhiteSpace())
                    // 没有格式化输出,直接replace
                {
                    //value = value.Replace(item.Value, v?.ToString());
                    sb.Append(v ?? string.Empty);
                    lastIndex += item.Value.Length;
                }
                else
                {
                    var hasFormater = false; // 有没有格式化器
                    // 正则匹配,计算action
                    var formatMatch = formatExpressionReg.Match(formatExp!);
                    if (formatMatch.Success && formatMatch.Groups.Count >= 2)
                    {
                        var action = formatMatch.Groups[1].Value;
                        var formater = formatters.GetOrDefault(action);
                        if (formater != null)
                        {
                            var expression = formatExp.TrimStart(formater.Action.ToCharArray()).Trim().TrimStart('(')
                                .TrimEnd(')').Trim();
                            var valueStr = v?.ToString();
                            var s = formater.Format(valueStr, expression);
                            //value = value.Replace(item.Value, s ?? "");
                            sb.Append(s ?? string.Empty);
                            lastIndex += item.Value.Length;
                            hasFormater = true;
                        }
                    }

                    if (!hasFormater)
                    {
                        // 没有格式化器,使用标准string format-
                        var s = string.Format($"{{0:{formatExp}}}", v);
                        //value = value.Replace(item.Value, s);
                        sb.Append(s);
                        lastIndex += item.Value.Length;
                    }
                }
            }

            if (lastIndex < inputSpan.Length)
                sb.Append(inputSpan.Slice(lastIndex));

            return sb.ToString();
        }
    }
}