using System;
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
            int byteLength = 1;
            byteLength *= (length / 3 + 1);
            // Create a byte array to hold the random value.  
            byte[] randomNumber = new byte[byteLength];
            // Create a new instance of the RNGCryptoServiceProvider.  
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            // Fill the array with a random value.  
            rng.GetBytes(randomNumber);
            // Convert the byte to an uint value to make the modulus operation easier.  
            uint randomResult = 0x0;
            for (int i = 0; i < byteLength; i++)
            {
                randomResult |= ((uint)randomNumber[i] << ((byteLength - 1 - i) * 8));
            }
            string s = randomResult.ToString();
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
        public static int[] GetRandomNum(int num, int minValue, int maxValue)
        {
            Random ra = new Random(unchecked((int)DateTime.Now.Ticks));//保证产生的数字的随机性
            int[] arrNum = new int[num];
            for (int i = 0; i <= num - 1; i++)
            {
                int tmp = ra.Next(minValue, maxValue);
                arrNum[i] = GetNum(arrNum, tmp, minValue, maxValue, ra); //取出值赋到数组中 
            }
            return arrNum;
        }

        /// <summary>
        /// 取出值赋到数组中 
        /// </summary>
        /// <param name="arrNum"></param>
        /// <param name="tmp"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="ra"></param>
        /// <returns></returns>
        static int GetNum(int[] arrNum, int tmp, int minValue, int maxValue, Random ra)
        {
            int n = 0;
            while (n > arrNum.Length - 1)
            {
                if (arrNum[n] == tmp) //利用循环判断是否有重复
                {
                    tmp = ra.Next(minValue, maxValue); //重新随机获取。
                    GetNum(arrNum, tmp, minValue, maxValue, ra); //递归:如果取出来的数字和已取得的数字有重复就重新随机获取。
                }
                n++;
            }
            return tmp;
        }
    }
}
