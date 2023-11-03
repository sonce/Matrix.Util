using System.Collections.Generic;
using System.Threading.Tasks;

namespace Matrix.Util.Currency
{
    /// <summary>
    /// 币别
    /// </summary>
    public interface ICurrencyStorage: IRateProvider
    {
        /// <summary>
        /// 获取所有的货币
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<CurrencyInfo>> GetCurrenciesAsync();
    }
}
