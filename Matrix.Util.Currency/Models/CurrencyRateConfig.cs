using System;
using System.Collections.Generic;
using System.Text;

namespace Matrix.Util.Currency
{
    /// <summary>
    /// 货币汇率配置
    /// </summary>
    public class CurrencyRateConfig
    {
        /// <summary>
        /// 请求API接口的Key
        /// </summary>
        public string ApiKey { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;
    }
}
