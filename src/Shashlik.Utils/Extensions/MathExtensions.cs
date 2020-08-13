using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Guc.Utils.Extensions
{
    /// <summary>
    /// 数学相关扩展类
    /// </summary>
    public static class MathExtensions
    {
        /// <summary>
        /// 小数保留精度并向上取整
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>        
        /// <returns></returns>
        public static decimal RoundCeil(this decimal value, int precision)
        {
            // 保留percison位小数 并向下取整
            return Math.Ceiling((value * (decimal)Math.Pow(10, precision))) / (decimal)Math.Pow(10, precision);
        }

        /// <summary>
        /// 小数保留精度并向下取整
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>        
        /// <returns></returns>
        public static decimal RoundFloor(this decimal value, int precision)
        {
            // 保留percison位小数 并向下取整
            return Math.Floor((value * (decimal)Math.Pow(10, precision))) / (decimal)Math.Pow(10, precision);
        }

        /// <summary>
        /// 小数保留精度并向上取整
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>        
        /// <returns></returns>
        public static float RoundCeil(this float value, int precision)
        {
            // 保留percison位小数 并向下取整
            return (float)Math.Ceiling((value * (float)Math.Pow(10, precision))) / (float)Math.Pow(10, precision);
        }

        /// <summary>
        /// 小数保留精度并向下取整
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>        
        /// <returns></returns>
        public static float RoundFloor(this float value, int precision)
        {
            // 保留percison位小数 并向下取整
            return (float)Math.Floor((value * (float)Math.Pow(10, precision))) / (float)Math.Pow(10, precision);
        }

        /// <summary>
        /// 小数保留精度并向上取整
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>        
        /// <returns></returns>
        public static double RoundCeil(this double value, int precision)
        {
            // 保留percison位小数 并向下取整
            return Math.Ceiling((value * (double)Math.Pow(10, precision))) / (double)Math.Pow(10, precision);
        }

        /// <summary>
        /// 小数保留精度并向下取整
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>        
        /// <returns></returns>
        public static double RoundFloor(this double value, int precision)
        {
            // 保留percison位小数 并向下取整
            return Math.Floor((value * (double)Math.Pow(10, precision))) / (double)Math.Pow(10, precision);
        }

        /// <summary>
        /// 保留<paramref name="precision"/>位小数,默认AwayFromZero模式,中国式 四舍五入,ToEven银行家算法
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>        
        /// <returns></returns>
        public static float MathRound(this float value, int precision, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return (float)Math.Round(value, precision, mode);
        }

        /// <summary>
        /// 保留<paramref name="precision"/>位小数,中国式 四舍五入,ToEven银行家算法
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>        
        /// <returns></returns>
        public static double MathRound(this double value, int precision, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return (double)Math.Round(value, precision, mode);
        }

        /// <summary>
        /// 保留<paramref name="precision"/>位小数,中国式 四舍五入,ToEven银行家算法
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>        
        /// <returns></returns>
        public static decimal MathRound(this decimal value, int precision, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return (decimal)Math.Round(value, precision, mode);
        }

        /// <summary>
        /// 小数是否保留了<paramref name="precision"/>位精度
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>        
        /// <returns></returns>
        public static bool IsMaxPrecision(this decimal value, int precision)
        {
            return Math.Round(value, precision) == value;
        }

        /// <summary>
        /// 小数是否保留了<paramref name="precision"/>位精度
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>        
        /// <returns></returns>
        public static bool IsMaxPrecision(this float value, int precision)
        {
            return Math.Round(value, precision) == value;
        }

        /// <summary>
        /// 小数是否保留了<paramref name="precision"/>位精度
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>        
        /// <returns></returns>
        public static bool IsMaxPrecision(this double value, int precision)
        {
            return Math.Round(value, precision) == value;
        }
    }
}