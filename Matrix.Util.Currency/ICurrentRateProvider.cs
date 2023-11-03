using System.Threading.Tasks;

namespace Matrix.Util.Currency
{
    public interface ICurrentRateProvider: IRateProvider
    {
        /// <summary>
        /// 获取货币间的汇率
        /// </summary>
        /// <param name="fromCurrencyCode">源货币</param>
        /// <param name="toCurrencyCode">目标货币</param>
        /// <returns></returns>
        Task<CurrencyRate> GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode);

        ///// <summary>
        ///// 获取货币的所有汇率
        ///// </summary>
        ///// <param name="currencyCode">货币</param>
        ///// <returns></returns>
        //Task<CurrencyRates> GetCurrencyRates(string currencyCode);

        /// <summary>
        /// 计算汇率
        /// </summary>
        /// <param name="fromCurrencyCode">原币别</param>
        /// <param name="toCurrencyCode">目标货币</param>
        /// <param name="money">金额</param>
        /// <returns></returns>
        Task<decimal> Calc(string fromCurrencyCode, string toCurrencyCode, decimal money);
    }
}
