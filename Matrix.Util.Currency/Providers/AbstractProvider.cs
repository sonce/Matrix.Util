using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Matrix.Util.Extentions;

namespace Matrix.Util.Currency.Providers
{
    /// <summary>
    /// https://www.abstractapi.com/
    /// 未提供货币列表API
    /// </summary>
    public class AbstractProvider : BaseHistoricalRateProvider
    {
        public override async Task<CurrencyRate> GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode, DateTime? date = null)
        {
            string url;
            if (date.HasValue)
                url = $"https://exchange-rates.abstractapi.com/v1/historical?api_key={Config.ApiKey}&base={fromCurrencyCode}&target={toCurrencyCode}&date={date.Value.ToString("yyyy-MM-dd")}";
            else
                url = $"https://exchange-rates.abstractapi.com/v1/live?api_key={Config.ApiKey}&base={fromCurrencyCode}&target={toCurrencyCode}";

            var jobj = await Utility.GetApiResult(url);
            if (jobj == null)
                return null;
            JObject jRates = jobj.Value<JObject>("exchange_rates");
            CurrencyRate rate = new CurrencyRate();
            rate.FromCurrency = fromCurrencyCode;
            rate.ToCurrency = toCurrencyCode;
            if (jobj.ContainsKey("date"))
                rate.Date = jobj.Value<DateTime>("date");
            else
                rate.Date = jobj.Value<double>("last_updated").ToDateTime();
            foreach (var item in jRates)
            {
                rate.Rate = decimal.Parse(item.Value.ToString());
            }
            if (rate.Rate == 0)
                return null;
            return rate;

        }
    }
}
