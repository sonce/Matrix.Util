using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Util.Currency.Providers
{
    /// <summary>
    /// https://currency.getgeoapi.com/
    /// </summary>
    public class ApiplantProvider : BaseHistoricalRateProvider, ICurrencyStorage
    {
        public async Task<IEnumerable<CurrencyInfo>> GetCurrenciesAsync()
        {
            string url = $"https://api.getgeoapi.com/v2/currency/list?api_key={Config.ApiKey}";
            JObject jResult = await Utility.GetApiResult(url);
            if (jResult == null || !jResult.ContainsKey("currencies"))
                return Enumerable.Empty<CurrencyInfo>();
            JObject jCurrencies = jResult.Value<JObject>("currencies");
            IList<CurrencyInfo> results = new List<CurrencyInfo>();
            foreach (var item in jCurrencies)
            {
                results.Add(new CurrencyInfo() { Code = item.Key, EnName = item.Value.ToString() });
            }
            return results;
        }

        public override async Task<CurrencyRate> GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode, DateTime? date = null)
        {
            string url = string.Empty;
            bool isLatest = !date.HasValue;
            if (date.HasValue && date.Value.Date != DateTime.Now.Date)
                url = $"https://api.getgeoapi.com/v2/currency/historical/{date.Value.ToString("yyyy-MM-dd")}?api_key={Config.ApiKey}&from={fromCurrencyCode}&to={toCurrencyCode}&amount=1&format=json";
            else
                url = $"https://api.getgeoapi.com/v2/currency/convert?api_key={Config.ApiKey}&from={fromCurrencyCode}&to={toCurrencyCode}&amount=1&format=json";
            JObject jResult = await Utility.GetApiResult(url);
            if (jResult != null && jResult.Value<string>("status") == "success")
            {
                JObject jRates = jResult.Value<JObject>("rates");
                foreach (var jRate in jRates)
                {
                    CurrencyRate rate = new CurrencyRate();
                    rate.FromCurrency = fromCurrencyCode;
                    rate.ToCurrency = toCurrencyCode;
                    rate.Date = jResult.Value<DateTime>("updated_date");
                    rate.Rate = decimal.Parse(jRate.Value["rate"].ToString());
                    return rate;
                }
            }
            return null;
        }
    }
}
