using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Matrix.Util.Extentions;
using System.Threading.Tasks;

namespace Matrix.Util.Currency.Providers
{
    /// <summary>
    /// https://currencylayer.com/
    /// </summary>
    public class CurrencylayerProvider : BaseHistoricalRateProvider, ICurrencyStorage
    {
        public async Task<IEnumerable<CurrencyInfo>> GetCurrenciesAsync()
        {
            string url = $"http://api.currencylayer.com/list?access_key={Config.ApiKey}";
            var jobj = await Utility.GetApiResult(url);
            if (jobj == null)
                return Enumerable.Empty<CurrencyInfo>();

            bool isSucess = jobj.Value<bool>("success");
            if (isSucess)
            {
                JObject jcurrencies = jobj.Value<JObject>("currencies");
                IList<CurrencyInfo> results = new List<CurrencyInfo>();
                foreach (var item in jcurrencies)
                {
                    results.Add(new CurrencyInfo() { Code = item.Key, EnName = item.Value.ToString() });
                }
                return results;
            }
            return Enumerable.Empty<CurrencyInfo>();

        }

        public override async Task<CurrencyRate> GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode, DateTime? date = null)
        {
            string url;
            if (date.HasValue)
                url = $"http://api.currencylayer.com/historical?access_key={Config.ApiKey}&date={date.Value.ToString("yyyy-MM-dd")}&source={fromCurrencyCode}&currencies={toCurrencyCode}";
            else
                url = $"http://api.currencylayer.com/live?access_key={Config.ApiKey}&source={fromCurrencyCode}&currencies={toCurrencyCode}";

            var jResult = await Utility.GetApiResult(url);
            bool isSucess = jResult.Value<bool>("success");
            if (isSucess)
            {
                JObject jcurrencies = jResult.Value<JObject>("quotes");

                CurrencyRate rate = new CurrencyRate();
                rate.FromCurrency = fromCurrencyCode;
                rate.ToCurrency = toCurrencyCode;
                rate.Date = jResult.Value<double>("timestamp").ToDateTime();
                foreach (var item in jcurrencies)
                {
                    rate.Rate = decimal.Parse(item.Value.ToString());
                }
                return rate;
            }
            return null;
        }
    }
}
