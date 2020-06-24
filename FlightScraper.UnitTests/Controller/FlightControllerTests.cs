using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;
using Flightscraper.Spa.Conf;
using Flightscraper.Spa.Controllers;
using Flightscraper.Spa.Interfaces;
using FlightScraper.UnitTests.UnitTest_Start;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;

namespace FlightScraper.UnitTests.Controller
{
    [TestClass]
    public class FlightControllerTests
    {
        private static readonly Mock<IFlightRepository> MockIFlightRepository;
        private static readonly Mock<ILog> MockILog;

        static FlightControllerTests()
        {
            MockILog = new Mock<ILog>();
            MockILog.SetupAllProperties();

            UnitConfigInitializer.MockILoggerWrapper.Setup(_ => _.GetLogger(typeof(FlightController))).Returns(MockILog.Object);

            MockIFlightRepository = new Mock<IFlightRepository>();
            MockIFlightRepository.SetupAllProperties();
        }


        [TestMethod]
        public void GetFlights_WhenValidRequestCall_ReturnExpectedFlightList()
        {

            // Arrange
            IEnumerable<IFlightResponse> expectedSearchResults = new List<IFlightResponse>();
            MockIFlightRepository.Setup(_ => _.BrowseQuotes(It.IsAny<IFlightRequest>())).Returns(expectedSearchResults);
            var controller = SetUpFlightController();

            // Act
            IHttpActionResult actionResult = controller.GetFlights("MIA", "NYC", "2021-01-01");
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<IFlightResponse>>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreSame(expectedSearchResults, contentResult.Content);
        }

        [TestMethod]
        public void GetFlights_WhenInValidRequestCall_ReturnBadRequest()
        {
            // Arrange 
            IEnumerable<IFlightResponse> expectedSearchResults = new List<IFlightResponse>();
            MockIFlightRepository.Setup(_ => _.BrowseQuotes(It.IsAny<IFlightRequest>())).Returns(expectedSearchResults);
            var controller = SetUpFlightController();

            // Act
            IHttpActionResult actionResult = controller.GetFlights("MI", "NY", "2021-20-01"); //invalid origin, destination and depart date
            var modelStateResult = actionResult as InvalidModelStateResult;

            var task = actionResult.ExecuteAsync(new CancellationToken());
            task.Wait();
            var httpResponse = task.Result;

            // Assert
            Assert.IsNotNull(modelStateResult);
            Assert.IsFalse(modelStateResult.ModelState.IsValid);
            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [TestMethod]
        public void GetFlights_WhenException_ReturnInternalServerError()
        {
            // Arrange
            MockIFlightRepository.Setup(_ => _.BrowseQuotes(It.IsAny<IFlightRequest>())).Throws<Exception>();
            var controller = SetUpFlightController();

            // Act
            IHttpActionResult actionResult = controller.GetFlights("MIA", "NYC", "2021-01-01");

            var task = actionResult.ExecuteAsync(new CancellationToken());
            task.Wait();
            var httpResponse = task.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, httpResponse.StatusCode);
            MockILog.Verify(_ => _.Error(It.IsAny<string>()), Times.Once);
        }

        private static FlightController SetUpFlightController()
        {
            FlightController controller = new FlightController(MockIFlightRepository.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            return controller;
        }
    }
}
