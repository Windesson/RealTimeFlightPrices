using System;
using Flightscraper.Spa.Interfaces;
using Flightscraper.Spa.Models;
using log4net;

namespace Flightscraper.Spa.Utilities
{
    public class LoggerWrapper : ILoggerWrapper
    {
        public ILog GetLogger(Type type)
        {
          return LogManager.GetLogger(type);
        }
    }
}