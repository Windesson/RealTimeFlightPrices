using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flightscraper.Spa.Conf;
using Flightscraper.Spa.Data;
using Flightscraper.Spa.Interfaces;
using Flightscraper.Spa.Models;
using FlightScraper.UnitTests.UnitTest_Start;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using log4net;

namespace FlightScraper.UnitTests.Data
{
    [TestClass]
    public class FlightRepositoryTest
    {
        private static Mock<ILoggerWrapper> mockILoggerWrapper;
        private static readonly Mock<ILog> mockILog;
        private static readonly Mock<IFlightResponseReader> mockKayakServices;
        private static readonly Mock<IFlightResponseReader> mockSkyscannerServices;

        static FlightRepositoryTest()
        {
            mockILog = new Mock<ILog>();
            mockILog.SetupAllProperties();

            UnitConfigInitializer.MockILoggerWrapper.Setup(_ => _.GetLogger(typeof(FlightRepository))).Returns(mockILog.Object);

            //kayak 
            mockKayakServices = new Mock<IFlightResponseReader>();
            mockKayakServices.SetupAllProperties();

            //skyscanner
            mockSkyscannerServices = new Mock<IFlightResponseReader>();
            mockSkyscannerServices.SetupAllProperties();
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockILog.Invocations.Clear();
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Constructor_WhenNull_ThrowException()
        {
            //act 
            var _ = new FlightRepository(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Constructor_WhenEmpty_ThrowException()
        {
            //act
            var _ =  new FlightRepository(new List<IFlightResponseReader>());
        }

        [TestMethod]
        public void GetQuotes_WhenNull_ReturnEmptyList()
        {
            //arrange 
            var repository = new FlightRepository(new List<IFlightResponseReader>(){ mockKayakServices.Object, mockSkyscannerServices.Object });

            //act 
            IEnumerable<IFlightResponse> responses = repository.BrowseQuotes(null);

            //assert
            Assert.IsTrue(!responses.Any());
            mockILog.Verify(_ => _.Error(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void GetQuotes_WhenCall_ReturnQuotes()
        {
            //arrange
            IFlightRequest myFlightRequest = new FlightRequest();

            IFlightResponse aKayakFlight = new FlightResponse();
            var kayakFlights = new List<IFlightResponse>(){ aKayakFlight };
            mockKayakServices.Setup(_ => _.BrowseQuotesAsync(myFlightRequest)).Returns(Task.FromResult(kayakFlights));

            IFlightResponse aSkyscannerFlight = new FlightResponse();
            var skyscannerFlights = new List<IFlightResponse>(){ aSkyscannerFlight };
            mockSkyscannerServices.Setup(_ => _.BrowseQuotesAsync(myFlightRequest)).Returns(Task.FromResult(skyscannerFlights));

            //act 
            var repository = new FlightRepository(new List<IFlightResponseReader>(){ mockKayakServices.Object, mockSkyscannerServices.Object });
            IEnumerable<IFlightResponse> quoteResponses = repository.BrowseQuotes(myFlightRequest);

            var expectedResponses = new List<IFlightResponse>() {aKayakFlight, aSkyscannerFlight};
            var expectedLogInfCount = 6;

            //assert
            Assert.IsNotNull(quoteResponses);
            Assert.AreEqual(quoteResponses.Count(), expectedResponses.Count);
            Assert.IsTrue(quoteResponses.Contains(expectedResponses[0]));
            Assert.IsTrue(quoteResponses.Contains(expectedResponses[1]));

            //assert log information
            mockILog.Verify(_ => _.Info(It.IsAny<string>()), Times.Exactly(expectedLogInfCount));

            //assert no log error
            mockILog.Verify(_ => _.Error(It.IsAny<string>()), Times.Never);
        }


    }
}
