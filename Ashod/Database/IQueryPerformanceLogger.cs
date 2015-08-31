using System;

namespace Ashod.Database
{
    public interface IQueryPerformanceLogger
    {
        void OnQueryEnd(string commandText, TimeSpan timeSpan);
    }
}
