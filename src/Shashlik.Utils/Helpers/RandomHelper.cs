using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable ClassNeverInstantiated.Global

namespace Shashlik.Utils.Helpers
{
    /// <summary>
    /// 随机函数
    /// </summary>
    public class RandomHelper
    {
        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="length">随机数长度</param>
        /// <returns>随机数</returns>
        public static string GetRandomCode(int length)
        {
            var byteLength = 1;
            byteLength *= (length / 3 + 1);
            // Create a byte array to hold the random value.  
            var randomNumber = new byte[byteLength];
            // Create a new instance of the RNGCryptoServiceProvider.  
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            // Fill the array with a random value.  
            rng.GetBytes(randomNumber);
            // Convert the byte to an uint value to make the modulus operation easier.  
            uint randomResult = 0x0;
            for (var i = 0; i < byteLength; i++)
            {
                randomResult |= ((uint) randomNumber[i] << ((byteLength - 1 - i) * 8));
            }

            var s = randomResult.ToString();
            if (s.Length > length)
                s = s.Substring(0, length);
            while (s.Length < length)
            {
                s = "0" + s;
            }

            return s;
        }

        /// <summary>
        /// 在区间[minValue,maxValue]取出num个互不相同的随机数，返回数组。
        /// </summary>
        /// <param name="num"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static List<int> GetRandomNum(int num, int minValue, int maxValue)
        {
            if (maxValue - minValue < num)
            {
                throw new ArgumentException("no enough number to generate");
            }

            var ra = new Random(unchecked((int) DateTime.Now.Ticks)); //保证产生的数字的随机性
            var numberList = new List<int>();
            while (numberList.Count < num)
            {
                var number = ra.Next(minValue, maxValue);
                if (numberList.Contains(number)) continue;
                numberList.Add(number);
            }

            return numberList;
        }
    }
}