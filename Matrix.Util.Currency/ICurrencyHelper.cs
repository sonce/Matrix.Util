using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Matrix.Util.Currency
{
    /// <summary>
    /// 货币
    /// </summary>
    public interface ICurrencyHelper
    {

        /// <summary>
        /// 获取货币间的汇率
        /// </summary>
        /// <param name="fromCurrencyCode">源货币</param>
        /// <param name="toCurrencyCode">目标货币</param>
        /// <param name="date">时间。如果时间为空，则为最新的汇率</param>
        /// <returns></returns>
        Task<CurrencyRate> GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode, DateTime? date = null);
        ///// <summary>
        ///// 获取货币的所有汇率
        ///// </summary>
        ///// <param name="currencyCode">货币</param>
        ///// <param name="date">时间。如果时间为空，则为最新的汇率</param>
        ///// <returns></returns>
        //Task<CurrencyRates> GetCurrencyRates(string currencyCode, DateTime? date = null);

        /// <summary>
        /// 计算汇率
        /// </summary>
        /// <param name="fromCurrencyCode">原币别</param>
        /// <param name="toCurrencyCode">目标货币</param>
        /// <param name="money">金额</param>
        /// <param name="date">汇率日期，如果为空则为最新的汇率</param>
        /// <returns></returns>
        Task<decimal> Calc(string fromCurrencyCode, string toCurrencyCode, decimal money, DateTime? date = null);
    }
}