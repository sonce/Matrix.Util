using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Matrix.Util.Currency.Providers
{
    /// <summary>
    /// https://github.com/fawazahmed0/currency-api
    /// </summary>
    public class CDNRateProvider : BaseHistoricalRateProvider,ICurrencyStorage
    {
        /// <summary>
        /// 获取所有的货币
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<CurrencyInfo>> GetCurrenciesAsync()
        {
            //https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies.json
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync("https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies.json");
                if (response.IsSuccessStatusCode)
                {
                    string strResult = await response.Content.ReadAsStringAsync();
                    Dictionary<string, string> dicResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(strResult);
                    IList<CurrencyInfo> results = new List<CurrencyInfo>();
                    foreach (var item in dicResult)
                    {
                        results.Add(new CurrencyInfo() { Code = item.Key, EnName = item.Value });
                    }
                    return results;
                }
                else
                    return Enumerable.Empty<CurrencyInfo>();
            }
        }

        ///// <summary>
        ///// 获取货币的所有汇率
        ///// </summary>
        ///// <param name="currencyCode">货币</param>
        ///// <param name="date">时间。如果时间为空，则为最新的汇率</param>
        ///// <returns></returns>
        //public override async Task<CurrencyRates> GetCurrencyRates(string currencyCode, DateTime? date = null)
        //{
        //    using (HttpClient httpClient = new HttpClient())
        //    {
        //        currencyCode = currencyCode.ToLower();
        //        string requestApiUrl = string.Format("https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/{0}/currencies/{1}.json", GetDateQueryPara(date), currencyCode);
        //        HttpResponseMessage response = await httpClient.GetAsync(requestApiUrl);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            string strResult = await response.Content.ReadAsStringAsync();

        //            try
        //            {

        //                JToken token = JObject.Parse(strResult);

        //                CurrencyRates rates = new CurrencyRates();
        //                rates.Date = token.Value<DateTime>("date");
        //                rates.CurrencyCode = currencyCode;
        //                JObject jRates = token.Value<JObject>(currencyCode);
        //                foreach (var item in jRates)
        //                {
        //                    rates.Rates.Add(new OfTheRate(item.Key, (decimal)item.Value));
        //                }
        //                return rates;
        //            }
        //            catch (Exception ex)
        //            {
        //                return null;
        //            }
        //        }
        //        else
        //            return null;
        //    }
        //}

        /// <summary>
        /// 获取货币间的汇率
        /// </summary>
        /// <param name="fromCurrencyCode">源货币</param>
        /// <param name="toCurrencyCode">目标货币</param>
        /// <param name="date">时间。如果时间为空，则为最新的汇率</param>
        /// <returns></returns>
        public override async Task<CurrencyRate> GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode, DateTime? date = null)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                fromCurrencyCode = fromCurrencyCode.ToLower();
                toCurrencyCode = toCurrencyCode.ToLower();
                string requestApiUrl = string.Format("https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/{0}/currencies/{1}/{2}.json", GetDateQueryPara(date), fromCurrencyCode, toCurrencyCode);
                HttpResponseMessage response = await httpClient.GetAsync(requestApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string strResult = await response.Content.ReadAsStringAsync();

                    try
                    {

                        JToken token = JObject.Parse(strResult);

                        CurrencyRate rate = new CurrencyRate();
                        rate.FromCurrency = fromCurrencyCode;
                        rate.ToCurrency = toCurrencyCode;
                        rate.Date = token.Value<DateTime>("date");
                        //if (rate.Date != date.Value.Date)
                            //return null;
                        rate.Rate = token.Value<decimal>(toCurrencyCode.ToLower());
                        return rate;
                    }
                    catch
                    {
                        return null;
                    }
                }
                else
                    return null;
            }
        }

        private static string GetDateQueryPara(DateTime? date)
        {
            if (!date.HasValue || date.Value.Date == DateTime.Today)
                return "latest";
            else
                return date.Value.ToString("yyyy-MM-dd");
        }
    }
}
