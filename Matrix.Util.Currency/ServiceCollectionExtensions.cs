using Matrix.Util.Currency.Providers;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;

namespace Matrix.Util.Currency
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加预置的货币提供器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPresetCurrency(this IServiceCollection services)
        {
            return services.AddCurrency(x => x.AddProvider<FXRateProvider>()
            .AddProvider<CDNRateProvider>()
                .AddProvider<XEProvider>());
        }

        public static IServiceCollection AddCurrency(this IServiceCollection services, RateBuilder builder)
        {
            //services.AddSingleton<NextCall<ICurrencyStorage>>();
            //services.AddSingleton<NextCall<ICurrentRateProvider>>();
            //services.AddSingleton<NextCall<IHistoricalRateProvider>>();

            services.AddSingleton<RateBuilder>(builder);
            foreach (var item in builder._currencyStorages)
            {
                services.AddSingleton<ICurrencyStorage>(item);
            }
            foreach (var item in builder._currenyRateProviders)
            {
                services.AddSingleton<ICurrentRateProvider>(item);
            }

            foreach (var item in builder._historicalRateProviders)
            {
                services.AddSingleton<IHistoricalRateProvider>(item);
            }

            return services;
        }

        public static IServiceCollection AddCurrency(this IServiceCollection services, Action<IRateBuilder> configAction)
        {
            RateBuilder rateBuilder = new RateBuilder();
            configAction(rateBuilder);
            return AddCurrency(services, rateBuilder);
        }
    }
}
