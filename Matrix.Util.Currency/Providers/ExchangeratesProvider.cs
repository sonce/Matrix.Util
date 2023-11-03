using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Matrix.Util.Extentions;
using System.Threading.Tasks;

namespace Matrix.Util.Currency.Providers
{
    /// <summary>
    /// https://manage.exchangeratesapi.io/
    /// </summary>
    public class ExchangeratesProvider : ICurrencyStorage
    {
        public ExchangeratesProvider()
        {
            this.Config = new CurrencyRateConfig();
        }
        public CurrencyRateConfig Config { get; set; }

        public async Task<IEnumerable<CurrencyInfo>> GetCurrenciesAsync()
        {
            string url = $"http://api.exchangeratesapi.io/v1/symbols?access_key={Config.ApiKey}";
            JObject jResult = await Utility.GetApiResult(url);
            if(jResult == null||!jResult.Value<bool>("success"))
                return null;
            JObject jCurrencies=jResult.Value<JObject>("symbols");
            IList<CurrencyInfo> results = new List<CurrencyInfo>();
            foreach (var item in jCurrencies)
            {
                results.Add(new CurrencyInfo() { Code = item.Key, EnName = item.Value.ToString() });
            }
            return results;
        }

        public async Task<CurrencyRate> GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode, DateTime? date = null)
        {
            string url = string.Empty;
            bool isLatest = !date.HasValue;
            if (date.HasValue && date.Value.Date != DateTime.Now.Date)
                url = $"http://api.exchangeratesapi.io/v1/{date.Value.ToString("yyyy-MM-dd")}?access_key={Config.ApiKey}&base={fromCurrencyCode}&symbols={toCurrencyCode}";
            else
                url = $"http://api.exchangeratesapi.io/v1/latest?access_key={Config.ApiKey}&base={fromCurrencyCode}&symbols={toCurrencyCode}";
            JObject jResult = await Utility.GetApiResult(url);
            if (jResult != null && jResult.Value<bool>("success"))
            {
                JObject jRates = jResult.Value<JObject>("rates");
                foreach (var jRate in jRates)
                {
                    CurrencyRate rate = new CurrencyRate();
                    rate.FromCurrency = jResult.Value<string>("base");
                    rate.ToCurrency = jRate.Key;
                    rate.Date = jResult.Value<double>("timestamp").ToDateTime();
                    rate.Rate = decimal.Parse(jRate.Value["rate"].ToString());
                    return rate;
                }
            }
            return null;
        }
    }
}
