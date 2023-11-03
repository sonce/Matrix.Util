using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Util.Currency.Providers
{
    /// <summary>
    /// https://free.currencyconverterapi.com/free-api-key
    /// </summary>
    public class CurrencyconverterProvider : BaseHistoricalRateProvider, ICurrencyStorage
    {
        public async Task<IEnumerable<CurrencyInfo>> GetCurrenciesAsync()
        {
            string url = $"https://free.currconv.com/api/v7/currencies?apiKey={Config.ApiKey}";
            JObject jobj = await Utility.GetApiResult(url);
            if (!jobj.ContainsKey("results"))
                return null;
            JObject jCurrencies = jobj.Value<JObject>("results");
            IList<CurrencyInfo> results = new List<CurrencyInfo>();
            foreach (var item in jCurrencies)
            {
                results.Add(new CurrencyInfo() { Code = item.Key, EnName = item.Value["currencyName"].ToString() });
            }
            return results;
        }

        public override async Task<CurrencyRate> GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode, DateTime? date = null)
        {
            string url = string.Empty;
            bool isLatest =false;
            if (date.HasValue&&date.Value.Date!= DateTime.Now.Date)
                url = $"https://free.currconv.com/api/v7/convert?q={fromCurrencyCode}_{toCurrencyCode}&compact=ultra&date={date.Value.ToString("yyyy-MM-dd")}&apiKey={Config.ApiKey}";
            else
            {
                isLatest = true;
                url = $"https://free.currconv.com/api/v7/convert?q={fromCurrencyCode}_{toCurrencyCode}&compact=ultra&apiKey={Config.ApiKey}";
            }
            JObject jResult = await Utility.GetApiResult(url);
            if (jResult == null)
                return null;
            CurrencyRate rate = null;
            foreach (var item in jResult)
            {
                rate = new CurrencyRate();
                rate.FromCurrency = fromCurrencyCode;
                rate.ToCurrency = toCurrencyCode;
                if (isLatest)
                {
                    rate.Date = DateTime.Now;
                    rate.Rate = decimal.Parse(item.Value.ToString());
                }
                else
                {
                    var jRate = JsonConvert.DeserializeObject<JObject>(item.Value.ToString());
                    foreach (var kvRate in jRate)
                    {
                        rate.Date = DateTime.Parse(kvRate.Key);
                        rate.Rate = decimal.Parse(kvRate.Value.ToString());
                    }
                    
                }

                //rate.Date = jResult.Value<double>("timestamp").UnixTimeStampToDateTime();
                //foreach (var item in jcurrencies)
                //{
                //    rate.Rate = decimal.Parse(item.Value.ToString());
                //}
            }
            return rate;
        }
    }
}
