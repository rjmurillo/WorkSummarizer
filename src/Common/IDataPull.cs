using System;
using System.Collections.Generic;

namespace DataSources
{
    public interface IDataPull<T>
    {
        IEnumerable<T> PullData(DateTime startDateTime, DateTime endDateTime);
    }
}
