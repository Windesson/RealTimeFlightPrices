using Flightscraper.Spa;
using Flightscraper.Spa.Conf;
using Flightscraper.Spa.Interfaces;
using Flightscraper.Spa.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;

namespace FlightScraper.UnitTests.UnitTest_Start
{
    [TestClass]
    public class UnitConfigInitializer
    {
        public static Mock<ILoggerWrapper> MockILoggerWrapper;
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            UnityConfig.Container = new UnityContainer();
            UnityConfig.Container.RegisterType<IQuoteIdGenerator, QuoteIdGenerator>(TypeLifetime.Singleton);

            MockILoggerWrapper = new Mock<ILoggerWrapper>();
            MockILoggerWrapper.SetupAllProperties();

            UnityConfig.Container.RegisterInstance(MockILoggerWrapper.Object);
        }
    }
}
