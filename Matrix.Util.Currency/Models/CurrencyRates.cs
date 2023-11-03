using System;
using System.Collections.Generic;
using System.Text;

namespace Matrix.Util.Currency
{
    public class CurrencyRates
    {
        /// <summary>
        /// 汇率的时间
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 货币编码
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public List<OfTheRate> Rates
        {
            get; set;
        } = new List<OfTheRate>();
    }

    public class OfTheRate {

        public OfTheRate(string toCurrency,decimal rate)
        {
            this.ToCurrency = toCurrency;
            this.Rate = rate;
        }
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
