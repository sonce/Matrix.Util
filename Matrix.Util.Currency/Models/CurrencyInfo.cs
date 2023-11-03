using System;

namespace Matrix.Util.Currency
{
    /// <summary>
    /// 货币
    /// </summary>
    public class CurrencyInfo
    {
        /// <summary>
        /// 货币代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 英文名字
        /// </summary>
        public string EnName { get; set; }
        public override string ToString()
        {
            return $"{Code}-{EnName}";
        }
    }
}
