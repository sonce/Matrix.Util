using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Util.Currency
{
    /// <summary>
    /// https://fixer.io/
    /// </summary>
    public class FixerProvider : BaseHistoricalRateProvider, ICurrencyStorage
    {
        public Task<IEnumerable<CurrencyInfo>> GetCurrenciesAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<CurrencyRate> GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode, DateTime? date = null)
        {
            throw new NotImplementedException();
        }
    }
}
