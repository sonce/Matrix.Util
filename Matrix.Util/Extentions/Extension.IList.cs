using System;
using System.Collections.Generic;
using System.Linq;

namespace Matrix.Util.Extentions
{
    public static partial class Extention
    {
        public static void Remove<T>(this IList<T> source, Type type)
        {
            var instances = source.Where(x => x?.GetType() == type).ToList();
            instances.ForEach(obj => source.Remove(obj));
        }
    }
}
