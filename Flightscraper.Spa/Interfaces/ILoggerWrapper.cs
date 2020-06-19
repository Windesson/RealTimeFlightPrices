using System;
using log4net;

namespace Flightscraper.Spa.Interfaces
{
    public interface ILoggerWrapper
    {
        ILog GetLogger(Type type);
    }
}