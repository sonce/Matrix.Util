using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Matrix.Util.Currency.Providers
{
    /// <summary>
    /// https://www.xe.com/
    /// </summary>
    public class XEProvider : BaseHistoricalRateProvider, ICurrencyStorage
    {
        public async Task<IEnumerable<CurrencyInfo>> GetCurrenciesAsync()
        {
            string url = "https://www.xe.com/";
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);
            string xPath = "//script[@id='__NEXT_DATA__']";
            var jsonNode = doc.DocumentNode.SelectSingleNode(xPath);
            string json = jsonNode.InnerText;
            var jobj = JsonConvert.DeserializeObject<JObject>(json);
            var jCurrencies = jobj["props"]["pageProps"]["commonI18nResources"]["currencies"].First.First.Value<JObject>();
            IList<CurrencyInfo> results = new List<CurrencyInfo>();
            foreach (var item in jCurrencies)
            {
                results.Add(new CurrencyInfo() { Code = item.Key.ToString(), EnName = item.Value["name"].ToString() });
            }
            return results;
        }

        public override async Task<CurrencyRate> GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode, DateTime? date = null)
        {
            string url = string.Empty;
            bool isLatest = true;
            if (date.HasValue && date.Value.Date != DateTime.Now.Date)
            {
                isLatest = false;
                url = $"https://www.xe.com/currencytables/?from={fromCurrencyCode.ToUpper()}&date={date.Value.ToString("yyyy-MM-dd")}";
            }
            else
                url = $"https://www.xe.com/currencyconverter/convert/?Amount=1&From={fromCurrencyCode.ToUpper()}&To={toCurrencyCode.ToUpper()}";

            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            if (isLatest)
            {
                var rateP=doc.DocumentNode.SelectSingleNode("//p[starts-with(@class,'result__BigRate')]");
                if (rateP != null)
                {
                    Regex reg = new Regex(@"\d+\.?\d*");
                    string strRate = reg.Match(rateP.InnerText).Value;
                    CurrencyRate rate = new CurrencyRate();
                    rate.FromCurrency = fromCurrencyCode;
                    rate.ToCurrency = toCurrencyCode;
                    rate.Date = DateTime.Now;
                    var tdRate = decimal.Parse(strRate);
                    rate.Rate = tdRate;
                    return rate;
                }
            }
            else
            {
                var trRates = doc.DocumentNode.SelectNodes("//table[starts-with(@class,\"currencytables\")]/tbody/tr");
                if(trRates != null)
                {
                    foreach (var trRate in trRates)
                    {
                        var thToCurrency = trRate.ChildNodes.FirstOrDefault(x => x.Name == "th");
                        if (thToCurrency != null)
                        {
                            if (string.Compare(thToCurrency.InnerText, toCurrencyCode, true) == 0)
                            {
                                CurrencyRate rate = new CurrencyRate();
                                rate.FromCurrency = fromCurrencyCode;
                                rate.ToCurrency = toCurrencyCode;
                                rate.Date = date.Value;
                                var tdRate = trRate.ChildNodes[2];
                                rate.Rate = decimal.Parse(tdRate.InnerText);
                                return rate;
                            }
                        }
                    }
                }
                
            }
            return null;
        }
    }
}
