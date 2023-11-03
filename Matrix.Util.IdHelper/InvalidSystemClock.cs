using System;
using System.Collections.Generic;
using System.Text;

namespace Matrix.Util
{
    internal class InvalidSystemClock : Exception
    {
        public InvalidSystemClock(string message) : base(message) { }
    }
}
