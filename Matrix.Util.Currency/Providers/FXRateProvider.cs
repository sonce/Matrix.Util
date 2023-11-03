using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Matrix.Util.Currency.Providers
{
    public class FXRateProvider : BaseHistoricalRateProvider, ICurrencyStorage
    {
        public async Task<IEnumerable<CurrencyInfo>> GetCurrenciesAsync()
        {
            //https://fx-rate.net/historical/
            var url = "https://fx-rate.net/historical/";
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);
            string xPath = "//select[@class='ip_currency_from']/following-sibling::option";
            var currencyNodes = doc.DocumentNode.SelectNodes(xPath);
            IList<CurrencyInfo> results = new List<CurrencyInfo>();

            foreach (HtmlNode currencyNode in currencyNodes)
            {
                results.Add(new CurrencyInfo() { Code = currencyNode.GetAttributeValue("value", ""), EnName = currencyNode.InnerText });
            }
            return results;
        }
        public override async Task<CurrencyRate> GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode, DateTime? date = null)
        {
            string url = string.Empty;

            string xfromPath = "//input[@name='amount_from']";
            string xToPath = "//input[@name='amount_to']";

            if (date.HasValue)
            {
                //https://fx-rate.net/ca.php?currency_pair=USDCNY&amount_from=1&amount_to=6.31&date_input=2022-03-03&interbank_input=0
                //https://fx-rate.net/calculator/?c_input=CNY&cp_input=USD&amount_from=1&date_input=2022-03-09
                url = $"https://fx-rate.net/calculator/?c_input={fromCurrencyCode}&cp_input={toCurrencyCode}&amount_from=1000&date_input={date.Value.ToString("yyyy-MM-dd")}";
            }
            else
            {
                //https://fx-rate.net/USD/CNY/
                url = $"https://fx-rate.net/{fromCurrencyCode}/{toCurrencyCode}/";
            }

            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            var fromInput = doc.DocumentNode.SelectSingleNode(xfromPath);
            var toInput = doc.DocumentNode.SelectSingleNode(xToPath);
            if (fromInput == null || toInput == null)
                return null;

            decimal fromVal = fromInput.GetAttributeValue<decimal>("value", 0);
            decimal toVal = toInput.GetAttributeValue<decimal>("value", 0);

            CurrencyRate rate = new CurrencyRate();
            rate.FromCurrency = fromCurrencyCode;
            rate.ToCurrency = toCurrencyCode;
            rate.Date = date ?? DateTime.Now;
            rate.Rate = Math.Round(toVal / fromVal, 4);
            if (rate.Rate == 0)
                return null;
            return rate;

        }

        //public override Task<CurrencyRates> GetCurrencyRates(string currencyCode, DateTime? date = null)
        //{
        //    throw new NotImplementedException();
        //    //https://fx-rate.net/CNY/?date_input=2022-03-01
        //}
    }
}
