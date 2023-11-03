using Microsoft.Extensions.DependencyInjection;
using System;

namespace Matrix.Util.Extentions
{
    public static partial class Extension
    {
        public static IHttpClientBuilder AddMatrixHttpClient<TClient, TImplementation>(this IServiceCollection services)
    where TClient : class
    where TImplementation : class, TClient
        {
            return services.AddMatrixHttpClient<TClient, TImplementation>(Guid.NewGuid().ToString(), null);
        }
        public static IHttpClientBuilder AddMatrixHttpClient<TClient, TImplementation>(this IServiceCollection services, Uri endpoint)
        where TClient : class
        where TImplementation : class, TClient
        {
            return services.AddMatrixHttpClient<TClient, TImplementation>(Guid.NewGuid().ToString(), endpoint);
        }
        public static IHttpClientBuilder AddMatrixHttpClient<TClient, TImplementation>(this IServiceCollection services, string name)
    where TClient : class
    where TImplementation : class, TClient
        {
            return services.AddMatrixHttpClient<TClient, TImplementation>(name, null);
        }

        public static IHttpClientBuilder AddMatrixHttpClient<TClient, TImplementation>(this IServiceCollection services, string name, Uri? endpoint)
        where TClient : class
        where TImplementation : class, TClient
        {
            IHttpClientBuilder clientBuilder;
            if (string.IsNullOrEmpty(name))
            {
                clientBuilder = services.AddHttpClient<TClient, TImplementation>(options =>
                {
                    if (endpoint != null)
                        options.BaseAddress = endpoint;
                });
            }
            else
            {
                clientBuilder = services.AddHttpClient<TClient, TImplementation>(name, options =>
                {
                    if (endpoint != null)
                        options.BaseAddress = endpoint;
                });
            }
            return clientBuilder;
        }
    }
}
