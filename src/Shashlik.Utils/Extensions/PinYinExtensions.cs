using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Guc.Utils.Extensions
{
    /// <summary>
    /// 汉字转拼音
    /// </summary>
    public static class PinYinExtensions
    {
        /// <summary>
        /// 汉字转全拼
        /// </summary>
        /// <param name="strChinese"></param>
        /// <returns></returns>
        public static string GetPinYin(this string strChinese)
        {
            if (strChinese.IsNullOrWhiteSpace())
                return null;
            try
            {
                if (strChinese.Length != 0)
                {
                    StringBuilder fullSpell = new StringBuilder();
                    for (int i = 0; i < strChinese.Length; i++)
                    {
                        var chr = strChinese[i];
                        fullSpell.Append(GetSpell(chr));
                    }

                    return fullSpell.ToString().ToUpper();
                }
            }
            catch
            {
                return string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// 汉字转全拼,仅保留字母和数字
        /// </summary>
        /// <param name="strChinese"></param>
        /// <returns></returns>
        public static string GetPinYinWithOnlyLetterAndNumbers(this string strChinese)
        {
            var p = strChinese.GetPinYin();
            return Regex.Replace(p, "[^A-Za-z0-9]", "");
        }

        /// <summary>
        /// 汉字转首字母
        /// </summary>
        /// <param name="strChinese"></param>
        /// <returns></returns>
        public static string GetPinYinFirst(this string strChinese)
        {
            if (strChinese.IsNullOrWhiteSpace())
                return null;
            try
            {
                if (strChinese.Length != 0)
                {
                    StringBuilder fullSpell = new StringBuilder();
                    for (int i = 0; i < strChinese.Length; i++)
                    {
                        var chr = strChinese[i];
                        fullSpell.Append(GetSpell(chr)[0]);
                    }

                    return fullSpell.ToString().ToUpper();
                }
            }
            catch
            {
                return string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// 汉字转首字母,仅保留字母和数字
        /// </summary>
        /// <param name="strChinese"></param>
        /// <returns></returns>
        public static string GetPinYinFirstWithOnlyLetterAndNumbers(this string strChinese)
        {
            var p = strChinese.GetPinYinFirst();
            return Regex.Replace(p, "[^A-Za-z0-9]", "");
        }

        private static string GetSpell(char chr)
        {
            var coverChr = NPinyin.Pinyin.GetPinyin(chr);

            bool isChinese = ChineseChar.IsValidChar(coverChr[0]);
            if (isChinese)
            {
                ChineseChar chineseChar = new ChineseChar(coverChr[0]);
                foreach (string value in chineseChar.Pinyins)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value.Remove(value.Length - 1, 1);
                    }
                }
            }

            return coverChr;

        }
    }
}
