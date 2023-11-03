using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Util.Currency
{
    public class RateBuilder : IRateBuilder, ICurrencyStorage, ICurrentRateProvider, IHistoricalRateProvider
    {
        private Dictionary<string, object> _currencies = new Dictionary<string, object>();
        public IList<IHistoricalRateProvider> _historicalRateProviders;
        public IList<ICurrencyStorage> _currencyStorages;
        public IList<ICurrentRateProvider> _currenyRateProviders;

        public CurrencyRateConfig Config { get; set; }

        public RateBuilder()
        {

        }

        public object[] GetProviders()
        {
            return _currencies.Values.ToArray();
        }

        public RateBuilder AddProvider<T>(T instance, CurrencyRateConfig config = null)
    where T : IRateProvider
        {
            string key = typeof(T).FullName;
            if (!_currencies.ContainsKey(key))
            {
                _historicalRateProviders = _historicalRateProviders ?? new List<IHistoricalRateProvider>();
                _currencyStorages = _currencyStorages ?? new List<ICurrencyStorage>();
                _currenyRateProviders = _currenyRateProviders ?? new List<ICurrentRateProvider>();
                instance.Config = config ?? new CurrencyRateConfig();
                _currencies.Add(key, instance);
                if (instance is ICurrencyStorage)
                {
                    _currencyStorages.Add(instance as ICurrencyStorage);
                }
                if (instance is IHistoricalRateProvider)
                {
                    _historicalRateProviders.Add(instance as IHistoricalRateProvider);
                }
                if (instance is ICurrentRateProvider)
                {
                    _currenyRateProviders.Add(instance as ICurrentRateProvider);
                }
            }

            return this;
        }
        public RateBuilder AddProvider<T>(CurrencyRateConfig config = null)
            where T : IRateProvider
        {
            string key = typeof(T).FullName;
            if (!_currencies.ContainsKey(key))
            {
                T instance = Activator.CreateInstance<T>();
                AddProvider<T>(instance, config);
            }

            return this;
        }

        public RateBuilder AddHistoricalRateProvider<T>(CurrencyRateConfig config = null)
            where T : IHistoricalRateProvider
        {
            return AddProvider<T>(config);
        }

        public RateBuilder AddCurrentRateProvider<T>(CurrencyRateConfig config = null)
            where T : ICurrentRateProvider
        {
            return AddProvider(Activator.CreateInstance<T>(), config);
        }

        public RateBuilder AddCurrencyStorage<T>(CurrencyRateConfig config = null)
            where T : ICurrencyStorage
        {
            AddProvider(Activator.CreateInstance<T>(), config);
            return this;
        }

        public async Task<IEnumerable<CurrencyInfo>> GetCurrenciesAsync()
        {
            NextCall<ICurrencyStorage> nextCall = new NextCall<ICurrencyStorage>(_currencyStorages.Where(x => x.Config.Enable));
            return await nextCall.ExcuteAsync<IEnumerable<CurrencyInfo>>(x => x.GetCurrenciesAsync(), pb => pb.OrResult<IEnumerable<CurrencyInfo>>(x => !x.Any()));
            //foreach (ICurrencyStorage storage in _currencyStorages.Where(x => x.Config.Enable))
            //{
            //    try
            //    {
            //        return await storage.GetCurrenciesAsync();
            //    }
            //    catch
            //    {
            //        continue;
            //    }
            //}

            //return Enumerable.Empty<CurrencyInfo>();
        }

        public Task<CurrencyRate> GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode)
        {
            return GetCurrencyRate(fromCurrencyCode, toCurrencyCode, null);
        }

        public Task<decimal> Calc(string fromCurrencyCode, string toCurrencyCode, decimal money)
        {
            return Calc(fromCurrencyCode, toCurrencyCode, money, null);
        }

        public async Task<CurrencyRate> GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode, DateTime? date = null)
        {

            if (date.HasValue && date < DateTime.Now)
            {
                NextCall<IHistoricalRateProvider> nextCall = new NextCall<IHistoricalRateProvider>(_historicalRateProviders.Where(x => x.Config.Enable));
                //nextCall.PolicyBuilder.OrResult<CurrencyRate>(x => x == null || x.Rate <= 0);
                return await nextCall.ExcuteAsync<CurrencyRate>(x => x.GetCurrencyRate(fromCurrencyCode, toCurrencyCode,date), pb => pb.OrResult<CurrencyRate>(x => x == null || x.Rate <= 0));
                //foreach (IHistoricalRateProvider provider in _historicalRateProviders.Where(x => x.Config.Enable))
                //{
                //    try
                //    {
                //        CurrencyRate rate = await provider.GetCurrencyRate(fromCurrencyCode, toCurrencyCode, date);
                //        if (rate.Rate <= 0)
                //            continue;
                //        else
                //            return rate;
                //    }
                //    catch
                //    {
                //        continue;
                //    }
                //}
            }
            else
            {
                NextCall<ICurrentRateProvider> nextCall = new NextCall<ICurrentRateProvider>(_currenyRateProviders.Where(x => x.Config.Enable));
                //nextCall.DefaultPolicyBuilder = nextCall.PolicyBuilder.OrResult<CurrencyRate>(x => x == null || x.Rate <= 0);
                return await nextCall.ExcuteAsync<CurrencyRate>(x => x.GetCurrencyRate(fromCurrencyCode, toCurrencyCode), pb => pb.OrResult<CurrencyRate>(x =>
                {
                    return x == null || x.Rate <= 0;
                }));
                //foreach (ICurrentRateProvider provider in _currenyRateProviders.Where(x => x.Config.Enable))
                //{
                //    try
                //    {
                //        CurrencyRate rate = await provider.GetCurrencyRate(fromCurrencyCode, toCurrencyCode);
                //        if (rate.Rate <= 0)
                //            continue;
                //        else
                //            return rate;
                //    }
                //    catch
                //    {
                //        continue;
                //    }
                //}
            }
        }

        public async Task<decimal> Calc(string fromCurrencyCode, string toCurrencyCode, decimal money, DateTime? date = null)
        {
            if (money <= 0)
                return money;

            if (date.HasValue && date < DateTime.Now)
            {
                NextCall<IHistoricalRateProvider> nextCall = new NextCall<IHistoricalRateProvider>(_historicalRateProviders.Where(x => x.Config.Enable));
                //nextCall.PolicyBuilder.OrResult<decimal>(x => x <= 0);
                return await nextCall.ExcuteAsync(x => x.Calc(fromCurrencyCode, toCurrencyCode, money, date), pb => pb.OrResult<decimal>(x => x <= 0));
                //foreach (IHistoricalRateProvider provider in _historicalRateProviders.Where(x => x.Config.Enable))
                //{
                //    try
                //    {
                //        return await provider.Calc(fromCurrencyCode, toCurrencyCode, money, date);
                //    }
                //    catch
                //    {
                //        continue;
                //    }
                //}
            }
            else
            {
                NextCall<ICurrentRateProvider> nextCall = new NextCall<ICurrentRateProvider>(_currenyRateProviders.Where(x => x.Config.Enable));
                return await nextCall.ExcuteAsync(x => x.Calc(fromCurrencyCode, toCurrencyCode, money), pb => pb.OrResult<decimal>(x => x <= 0));
                //foreach (ICurrentRateProvider provider in _currenyRateProviders.Where(x => x.Config.Enable))
                //{
                //    try
                //    {
                //        return await provider.Calc(fromCurrencyCode, toCurrencyCode, money);
                //    }
                //    catch
                //    {
                //        continue;
                //    }
                //}
            }
        }
    }
}
