using System;
using System.Collections.Generic;
using System.Text;

namespace Matrix.Util.Currency
{
    /// <summary>
    /// 货币汇率
    /// </summary>
    public class CurrencyRate
    {
        /// <summary>
        /// 汇率的时间
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 源货币
        /// </summary>
        public string FromCurrency { get; set; }

        /// <summary>
        /// 目标货币
        /// </summary>
        public string ToCurrency { get; set; } 

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }
    }
}
