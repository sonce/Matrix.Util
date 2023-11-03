using System;
using System.Collections.Generic;
using System.Linq;

namespace Matrix.Util.Currency
{
    public interface IRateBuilder
    {
        public RateBuilder AddCurrencyStorage<T>(CurrencyRateConfig config = null)
            where T : ICurrencyStorage;

        public RateBuilder AddCurrentRateProvider<T>(CurrencyRateConfig config = null)
            where T : ICurrentRateProvider;

        public RateBuilder AddHistoricalRateProvider<T>(CurrencyRateConfig config = null)
            where T : IHistoricalRateProvider;

        public RateBuilder AddProvider<T>(T instance, CurrencyRateConfig config = null)
    where T : IRateProvider;

        public RateBuilder AddProvider<T>(CurrencyRateConfig config = null)
            where T : IRateProvider;

        public object[] GetProviders();
    }
}