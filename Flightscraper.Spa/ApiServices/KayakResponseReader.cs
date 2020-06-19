using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Flightscraper.Spa.Conf;
using Flightscraper.Spa.Interfaces;
using Flightscraper.Spa.Models;
using log4net;
using Unity;

namespace Flightscraper.Spa.ApiServices
{
    /// <summary>
    /// Read flights response from Kayak web page and parse to a common flight response
    /// </summary>
    public class KayakResponseReader : IFlightResponseReader
    {
        private static readonly ILog Logger;
        private const int MaxNumOfHttpSearchRetries = 3;

        static KayakResponseReader()
        {
            Logger = UnityConfig.Container.Resolve<ILoggerWrapper>().GetLogger(typeof(KayakResponseReader));
        }

        /// <summary>
        /// Browse for real-time flight quotes
        /// </summary>
        /// <param name="request">the user flight request</param>
        /// <returns></returns>
        public async Task<List<IFlightResponse>> BrowseQuotesAsync(IFlightRequest request)
        {
            var realTimeFlights = new List<IFlightResponse>();
            try
            {
                var numOfLodAttempts = 0;
                do //run query until result is found but not exceeding the maximum number of tries.
                {
                    await LoadQuotes(request, realTimeFlights);
                    numOfLodAttempts++;
                }
                while (ContinueSearch(realTimeFlights, numOfLodAttempts));

            }
            catch (Exception ex)
            {
                Logger.Error($"{ex.Message}|{ex.InnerException}");
            }

            return realTimeFlights;
        }

        #region private methods

        /// <summary>
        /// Load real-time flights prices 
        /// </summary>
        /// <param name="modelQuoteRequest">model representing the users flight request</param>
        /// <param name="flights">list of flight to append results to the end</param>
        /// <returns></returns>
        private async Task LoadQuotes(IFlightRequest modelQuoteRequest, List<IFlightResponse> flights)
        {
            var kayakLoader = new KayakLoader(modelQuoteRequest);

            IHtmlCollection<IElement> searchResults = await kayakLoader.GetFlightCollection();

            if (searchResults == null) return;
            foreach (IElement searchResult in searchResults)
            {
                var flight = new FlightResponse
                {
                    QuoteSource = "Kayak",
                    TripTotalPrice = KayakLoader.GetFlightCost(searchResult),
                    BookingLink = kayakLoader.RequestUrl,
                    FlightItineraries = KayakLoader.GetFlightItineraries(searchResult)
                };
           
                flights.Add(flight);
            }
            
        }

        /// <summary>
        ///  Continue search until a flight is found or the maximum number of tries have being reached.
        /// </summary>
        /// <param name="tripQuotes">list of real time flights</param>
        /// <param name="numOfTries">number if failed attempts to retrive data</param>
        /// <returns></returns>
        private static bool ContinueSearch(IEnumerable<IFlightResponse> tripQuotes, int numOfTries)
        {
            return !tripQuotes.Any() && numOfTries < MaxNumOfHttpSearchRetries;
        }

        #endregion
    }
}
