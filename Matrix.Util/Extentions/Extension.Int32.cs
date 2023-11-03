using System;

namespace Matrix.Util.Extentions
{
    public static partial class Extention
    {
        /// <summary>
        /// 向上整除
        /// 1.当num能被divideBy整除时,结果即为num/divideBy;
        /// 2.当num不能被divideBy整除时,结果为num/divideBy + 1;
        /// </summary>
        /// <param name="num">被除数,大于或者等于0</param>
        /// <param name="divideBy">除数,大于0</param>
        /// <returns>向上整除结果</returns>
        public static int CeilingDivide(this int num, int divideBy)
        {
            if (num < 0) throw new ArgumentException("num");
            if (divideBy <= 0) throw new ArgumentException("divideBy");

            return (num + divideBy - 1) / divideBy;
        }

        /// <summary>
        /// 转换成友好显示字符串
        /// </summary>
        /// <param name="number">数量</param>
        /// <returns></returns>
        public static string ToFriendlyNumber(this int number)
        {
            if (number <= 99)
            {
                return number.ToString();
            }
            return "99+";
        }
    }
}
