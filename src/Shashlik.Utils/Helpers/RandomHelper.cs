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
        public static readonly RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();

        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="length">随机数长度</param>
        /// <returns>随机数</returns>
        public static string GetRandomCode(int length)
        {
            int min = 1, max = 1;
            var ml = length > 9 ? 9 : length;
            for (var i = 1; i <= ml; i++)
            {
                max *= 10;
                if (i <= ml - 2)
                    min *= 10;
            }

            min -= 1;
            max -= 1;

            if (length <= 9)
                return Next(min, max).ToString($"D{length}").AsSpan()[0..length].ToString();

            var sb = new StringBuilder();
            int l;
            for (var i = 0; i < length; i += 9)
            {
                l = length - i;
                l = l > 9 ? 9 : l;
                sb.Append(Next(min, max).ToString($"D{l}").AsSpan()[0..l].ToString());
            }

            return sb.ToString();
        }

        public static int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue));
            
            if (minValue == maxValue) return minValue;
            long diff = maxValue - minValue;
            var bytes = new byte[4];
            while (true)
            {
                Rng.GetBytes(bytes);
                var rand = BitConverter.ToUInt32(bytes, 0);

                var max = 1 + (long) uint.MaxValue;
                var remainder = max % diff;
                if (rand < max - remainder)
                {
                    return (int) (minValue + rand % diff);
                }
            }
        }
    }
}