using System;
using System.Collections.Generic;
using System.Text;

namespace Matrix.Util.Currency
{
    public interface IRateProvider
    {
        CurrencyRateConfig Config { get; set; }
    }
}
