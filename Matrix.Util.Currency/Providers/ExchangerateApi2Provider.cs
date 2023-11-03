using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Matrix.Util.Extentions;
using System.Threading.Tasks;

namespace Matrix.Util.Currency.Providers
{
    /// <summary>
    /// https://www.exchangerate-api.com/
    /// </summary>
    public class ExchangerateApi2Provider : ICurrentRateProvider, ICurrencyStorage
    {
        public ExchangerateApi2Provider()
        {
            this.Config = new CurrencyRateConfig();
        }
        public CurrencyRateConfig Config { get; set; }

        public async Task<IEnumerable<CurrencyInfo>> GetCurrenciesAsync()
        {
            var url = $"https://v6.exchangerate-api.com/v6/{Config.ApiKey}/codes";
            var jobj = await Utility.GetApiResult(url);
            //if (!jobj.Value<string>("result"))
            //    return Enumerable.Empty<CurrencyInfo>();
            JArray jCurrencies = jobj.Value<JArray>("supported_codes");
            if (jCurrencies == null)
                return Enumerable.Empty<CurrencyInfo>();

            IList<CurrencyInfo> results = new List<CurrencyInfo>();
            foreach (var item in jCurrencies)
            {
                CurrencyInfo currencyInfo = new CurrencyInfo() { Code = item.First.ToString(), EnName = item.Last.ToString() };
                results.Add(currencyInfo);
            }
            return results;
        }

        private async Task<CurrencyRate> GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode, DateTime? date = null)
        {
            string url = string.Empty;
            bool isLatest = true;
            if (date.HasValue && date.Value.Date != DateTime.Now.Date)
            {
                //历史记录需要升级套餐，收费
                isLatest = false;
                url = $"https://v6.exchangerate-api.com/v6/{Config.ApiKey}/history/{fromCurrencyCode}/{date.Value.Year}/{date.Value.Month}/{date.Value.Day}";
            }
            else
                url = $"https://v6.exchangerate-api.com/v6/{Config.ApiKey}/pair/{fromCurrencyCode}/{toCurrencyCode}";

            JObject jResult = await Utility.GetApiResult(url);
            if (jResult!=null&&jResult.Value<string>("result") == "success")
            {
                CurrencyRate rate = new CurrencyRate();
                rate.FromCurrency = fromCurrencyCode;
                rate.ToCurrency = fromCurrencyCode;
                if (isLatest)
                {
                    rate.Date = jResult.Value<double>("time_last_update_unix").ToDateTime();
                    rate.Rate = jResult.Value<decimal>("conversion_rate");
                }
                else
                {
                    JObject jRates = jResult.Value<JObject>("conversion_rates");
                    JToken toRate = jRates[toCurrencyCode.ToUpper()];
                    rate.Date = date.Value;
                    if (toRate != null)
                        rate.Rate = decimal.Parse(toRate.ToString());
                }

                if (rate.Rate == 0)
                    return null;
                return rate;
            }
            return null;
        }

        public Task<CurrencyRate> GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode)
        {
            return this.GetCurrencyRate(fromCurrencyCode, toCurrencyCode, null);
        }


        public async Task<decimal> Calc(string fromCurrencyCode, string toCurrencyCode, decimal money)
        {
            if (string.Compare(fromCurrencyCode, toCurrencyCode, true) == 0)
                return money;
            CurrencyRate rate = await GetCurrencyRate(fromCurrencyCode, toCurrencyCode);
            return rate.Rate * money;
        }
    }
}
