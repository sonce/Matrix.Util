using System;
using System.Threading.Tasks;

namespace Matrix.Util.Currency
{
    public abstract class BaseHistoricalRateProvider : IHistoricalRateProvider, ICurrentRateProvider
    {
        public BaseHistoricalRateProvider()
        {
            Config = new CurrencyRateConfig();
        }
        public CurrencyRateConfig Config { get;set; }

        public async Task<decimal> Calc(string fromCurrencyCode, string toCurrencyCode, decimal money, DateTime? date = null)
        {
            if (string.Compare(fromCurrencyCode, toCurrencyCode, true) == 0)
                return money;
            CurrencyRate rate = await GetCurrencyRate(fromCurrencyCode, toCurrencyCode, date);
            return rate.Rate * money;
        }

        public Task<decimal> Calc(string fromCurrencyCode, string toCurrencyCode, decimal money)
        {
            return Calc(fromCurrencyCode, toCurrencyCode, money, null);
        }

        public abstract Task<CurrencyRate> GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode, DateTime? date = null);
        public virtual Task<CurrencyRate> GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode)
        {
            return GetCurrencyRate(fromCurrencyCode, toCurrencyCode, null);
        }

        //public abstract Task<CurrencyRates> GetCurrencyRates(string currencyCode, DateTime? date = null);
        //public virtual Task<CurrencyRates> GetCurrencyRates(string currencyCode)
        //{
        //    return GetCurrencyRates(currencyCode, null);
        //}
    }
}
