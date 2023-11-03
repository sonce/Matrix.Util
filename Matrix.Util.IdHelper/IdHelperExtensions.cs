using Microsoft.Extensions.Hosting;
using Matrix.Util.Extentions;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Matrix.Util.Extentions
{
    public static partial class Extention
    {
        /// <summary>
        /// 使用IdHelper
        /// </summary>
        /// <param name="hostBuilder">建造者</param>
        /// <returns></returns>
        public static IHostBuilder UseIdHelper(this IHostBuilder hostBuilder, Action<long> callback = null)
        {
            hostBuilder.ConfigureServices(delegate (HostBuilderContext buidler, IServiceCollection services)
            {
                long num = 0;
                if (buidler.Configuration["WorkerId"] != null)
                    num = buidler.Configuration["WorkerId"].ToLong();
                IdHelper.Init(num);
                if (callback != null)
                {
                    callback(num);
                }
            });
            return hostBuilder;
        }

    }
}
