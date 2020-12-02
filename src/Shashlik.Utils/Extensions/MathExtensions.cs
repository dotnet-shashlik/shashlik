using System;

namespace Shashlik.Utils.Extensions
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
            return Math.Ceiling(value * (decimal) Math.Pow(10, precision)) / (decimal) Math.Pow(10, precision);
        }

        /// <summary>
        /// 小数保留精度并向下取整
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>        
        /// <returns></returns>
        public static decimal RoundFloor(this decimal value, int precision)
        {
            return Math.Floor(value * (decimal) Math.Pow(10, precision)) / (decimal) Math.Pow(10, precision);
        }

        /// <summary>
        /// 小数保留精度并向上取整
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>        
        /// <returns></returns>
        public static float RoundCeil(this float value, int precision)
        {
            return (float) Math.Ceiling(value * (float) Math.Pow(10, precision)) / (float) Math.Pow(10, precision);
        }

        /// <summary>
        /// 小数保留精度并向下取整
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>        
        /// <returns></returns>
        public static float RoundFloor(this float value, int precision)
        {
            return (float) Math.Floor(value * (float) Math.Pow(10, precision)) / (float) Math.Pow(10, precision);
        }

        /// <summary>
        /// 小数保留精度并向上取整
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>        
        /// <returns></returns>
        public static double RoundCeil(this double value, int precision)
        {
            return Math.Ceiling(value * Math.Pow(10, precision)) / Math.Pow(10, precision);
        }

        /// <summary>
        /// 小数保留精度并向下取整
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>        
        /// <returns></returns>
        public static double RoundFloor(this double value, int precision)
        {
            return Math.Floor(value * Math.Pow(10, precision)) / Math.Pow(10, precision);
        }

        /// <summary>
        /// 保留<paramref name="precision"/>位小数,默认AwayFromZero模式,中国式 四舍五入,ToEven银行家算法
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>
        /// <param name="mode">舍入模式</param> 
        /// <returns></returns>
        public static float MathRound(this float value, int precision,
            MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return (float) Math.Round(value, precision, mode);
        }

        /// <summary>
        /// 保留<paramref name="precision"/>位小数,中国式 四舍五入,ToEven银行家算法
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>
        /// <param name="mode">舍入模式</param>
        /// <returns></returns>
        public static double MathRound(this double value, int precision,
            MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return Math.Round(value, precision, mode);
        }

        /// <summary>
        /// 保留<paramref name="precision"/>位小数,中国式 四舍五入,ToEven银行家算法
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="precision">精度</param>
        /// <param name="mode">舍入模式</param>
        /// <returns></returns>
        public static decimal MathRound(this decimal value, int precision,
            MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return Math.Round(value, precision, mode);
        }
    }
}