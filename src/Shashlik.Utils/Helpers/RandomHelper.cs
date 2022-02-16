using System;
using System.Security.Cryptography;
using System.Text;

// ReSharper disable ClassNeverInstantiated.Global

namespace Shashlik.Utils.Helpers
{
    /// <summary>
    /// 随机函数
    /// </summary>
    public static class RandomHelper
    {
        /// <summary>
        /// 获取随机数字(数字范围)
        /// </summary>
        /// <param name="length">随机数长度</param>
        /// <returns>随机数</returns>
        public static string RandomNumber(int length)
        {
            return RandomString(length, "0123456789");
        }

        /// <summary>
        /// 获取一个随机数
        /// </summary>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue));
            if (minValue == maxValue)
                return minValue;
            return RandomNumberGenerator.GetInt32(minValue, maxValue);
        }

        /// <summary>
        /// 获取随机字符
        /// </summary>
        /// <param name="length">字符长度</param>
        /// <param name="chars">候选字符集</param>
        /// <returns></returns>
        public static string RandomString(int length, string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890")
        {
            if (length <= 0)
                return "";
            if (string.IsNullOrWhiteSpace(chars)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(chars));

            byte[] random = RandomNumberGenerator.GetBytes(length);
            var res = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                var a = random[i];
                res.Append(chars[a % chars.Length]);
            }

            return res.ToString();
        }
    }
}