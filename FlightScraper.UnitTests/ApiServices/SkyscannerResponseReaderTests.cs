using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flightscraper.Spa.ApiServices;
using Flightscraper.Spa.Conf;
using Flightscraper.Spa.Interfaces;
using Flightscraper.Spa.Models;
using FlightScraper.UnitTests.UnitTest_Start;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;

namespace FlightScraper.UnitTests.ApiServices
{
    [TestClass]
    public class SkyscannerResponseReaderTests
    {
        private static readonly Mock<ILog> MockILog;
        private static readonly Mock<ISkyscannerLoader> MockISkyscannerLoader;

        static SkyscannerResponseReaderTests()
        {
            MockILog = new Mock<ILog>();
            MockILog.SetupAllProperties();

            UnitConfigInitializer.MockILoggerWrapper.Setup(_ => _.GetLogger(typeof(SkyscannerResponseReader))).Returns(MockILog.Object);

            MockISkyscannerLoader = new Mock<ISkyscannerLoader>();
            MockISkyscannerLoader.SetupAllProperties();

            UnityConfig.Container.RegisterInstance(MockISkyscannerLoader.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            MockILog.Invocations.Clear();
        }

        [TestMethod]
        public void BrowseQuotes_WhenCallNotQuotesFound_ReturnEmptyList()
        {
            // Arrange
            IFlightRequest request = new FlightRequest();
            var response = new SkyscannerBrowseQuoteResponse() { Quotes = new Quote[] { } };
            MockISkyscannerLoader.Setup(_ => _.BrowseQuotesAsync(request)).Returns(Task.FromResult(response));

            // Act
            var skyscannerQuoteReader = new SkyscannerResponseReader();
            Task<List<IFlightResponse>> taskResponse = skyscannerQuoteReader.BrowseQuotesAsync(request);
            taskResponse.Wait();

            var actualResponse = taskResponse.Result;

            // Assert
            Assert.IsNotNull(actualResponse);
            Assert.AreEqual(actualResponse.Count, 0);
            MockILog.Verify(_ => _.Error(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void BrowseQuotes_WhenCall_ReturnExpected()
        {
            //Arrange 
            var expectedDepartDate = "2020-08-01";
            var expectedInboundIataCode = "JFK";
            var expectedOutboundIataCode = "SFO";
            var expectedNumberOfStops = "1 stop";
            var expectedAirlineName = "Alaska Airlines";
            var expectedTripTotalPrice = "$139";
            var expectedSource = "Skyscanner";
            var expectedBookingBaseUrl = "https://www.kayak.com/flights";
            var expectedBookingUrl = $"{expectedBookingBaseUrl}/{expectedOutboundIataCode}/{expectedInboundIataCode}/{expectedDepartDate}/";

            IFlightRequest request = new FlightRequest(){DepartDate = expectedDepartDate, Destination = expectedInboundIataCode, Origin = expectedOutboundIataCode };
            SkyscannerBrowseQuoteResponse expectedResponse = GetAlaskaFlight();

            MockISkyscannerLoader.Setup(_ => _.BrowseQuotesAsync(request)).Returns(Task.FromResult(expectedResponse));
            MockISkyscannerLoader.Setup(_ => _.GetBaseBookingUrl()).Returns(expectedBookingBaseUrl);


            // Act
            var skyscannerQuoteReader = new SkyscannerResponseReader();
            Task<List<IFlightResponse>> taskResponse = skyscannerQuoteReader.BrowseQuotesAsync(request);
            taskResponse.Wait();

            List<IFlightResponse> actualResponses = taskResponse.Result;

            var expectedItineraries = 1; // 1 quote, and 1 Outbound, 0 Inbound 
            var actualItineraries = actualResponses[0].FlightItineraries;
            var actualResponse = actualResponses[0];

            //Assert
            Assert.IsNotNull(actualResponse);
            Assert.AreEqual(expectedResponse.Quotes.Length, actualResponses.Count);
            Assert.AreEqual(expectedSource, actualResponse.QuoteSource);
            Assert.AreEqual(expectedTripTotalPrice, actualResponse.TripTotalPrice);
            Assert.AreEqual(expectedBookingUrl, actualResponse.BookingLink);
            Assert.AreEqual(expectedItineraries, actualItineraries.Count);
            Assert.AreEqual(expectedAirlineName, actualItineraries[0].AirlineName);
            Assert.AreEqual(expectedInboundIataCode, actualItineraries[0].InboundIataCode);
            Assert.AreEqual(expectedOutboundIataCode, actualItineraries[0].OutboundIataCode);
            Assert.AreEqual(expectedNumberOfStops, actualItineraries[0].NumberOfStops);
        }

        private SkyscannerBrowseQuoteResponse GetAlaskaFlight()
        {
            return new SkyscannerBrowseQuoteResponse()
            {
                Quotes = new [] {
                    new Quote(){
                        QuoteId = 1,
                        MinPrice = "139",
                        Direct = true,
                        OutboundLeg = new Outboundleg()
                        {
                            CarrierIds = new []{ 851 },
                            OriginId = 81727,
                            DestinationId = 60987,
                        },
                     },
                },
                Places = new[]{ new Place(){
                    PlaceId = 60987,
                    IataCode = "JFK",
                    Name = "New York John F. Kennedy",
                    Type = "Station",
                    SkyscannerCode = "JFK",
                    CityName = "New York",
                    CityId = "NYCA",
                    CountryName = "United States",
                    },
                    new Place(){
                    PlaceId = 81727,
                    IataCode = "SFO",
                    Name = "San Francisco International",
                    Type = "Station",
                    SkyscannerCode = "SFO",
                    CityName = "San Francisco",
                    CityId = "SFOA",
                    CountryName = "United States",
                    }
                },
                Carriers = new[]
               {
                    new Carrier()
                    {
                    CarrierId = 851,
                    Name = "Alaska Airlines",
                    }
                }
            };
        }

    }
}