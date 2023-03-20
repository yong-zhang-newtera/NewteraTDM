using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Deedle;

namespace Newtera.MLServer.TimeSeries
{
    public enum TimeSeriesFrequency
    {
        None,
        Second,
        Minute,
        Hour,
        Day,
        Month
    }
}
