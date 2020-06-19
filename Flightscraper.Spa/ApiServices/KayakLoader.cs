using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Flightscraper.Spa.Conf;
using Flightscraper.Spa.Data;
using Flightscraper.Spa.Interfaces;
using Flightscraper.Spa.Models;
using log4net;
using Unity;

namespace Flightscraper.Spa.ApiServices
{
    /// <summary>
    /// Load real-time flights from Kayak.
    /// Parse Html document.
    /// Handle configuration dependecies.
    /// </summary>
    public class KayakLoader
    {
        private static readonly ILog Logger;
        private static readonly string KayakFlightsUrl;
        private static readonly IPageLoader DocumentLoader;
        private static readonly string KayakFlightUrlKey = "Kayak:flights:url";

        public string RequestUrl { get; }

        static KayakLoader()
        {
            KayakFlightsUrl = ConfigurationManager.AppSettings[KayakFlightUrlKey];
            Logger = UnityConfig.Container.Resolve<ILoggerWrapper>().GetLogger(typeof(KayakLoader));
            DocumentLoader = UnityConfig.Container.Resolve<IPageLoader>();
        }

        /// <summary>
        /// Initialize url request using user search detail. 
        /// </summary>
        /// <param name="modelQuoteRequest"></param>
        public KayakLoader(IFlightRequest modelQuoteRequest)
        {
            RequestUrl = GetKayakUriForSearch(modelQuoteRequest);
        }

        /// <summary>
        /// Fetch kayak page as Html document 
        /// </summary>
        /// <returns></returns>
        public async Task<IHtmlCollection<IElement>> GetFlightCollection()
        {
            using (var htmlDocument = await DocumentLoader.FetchHtmlDocument(RequestUrl))
            {
                return GetBestFlightResults(htmlDocument);
            }
        }

        /// <summary>
        /// The flights available in the HTML tagged as best deal.
        /// </summary>
        /// <param name="document">kayak web page document</param>
        /// <returns>collention of best flights</returns>
        public static IHtmlCollection<IElement> GetBestFlightResults(IHtmlDocument document)
        {
            return document.GetElementById("searchResultsList")?
                .QuerySelectorAll(".best-flights-list-results .resultWrapper");
        }

        /// <summary>
        ///  The URI used to perform the query
        /// </summary>
        /// <param name="modelQuoteRequest">User request</param>
        /// <returns>The Uri to perform flight request</returns>
        public string GetKayakUriForSearch(IFlightRequest modelQuoteRequest)
        {
            if (KayakFlightsUrl == null) throw new Exception($"AppSettings '{KayakFlightUrlKey}' found found.");
            var tripQuoteUrl = $"{KayakFlightsUrl}/{modelQuoteRequest.Origin}-{modelQuoteRequest.Destination}/" +
                $"{modelQuoteRequest.DepartDate}";

            if (modelQuoteRequest.ReturnDate != null)
            {
                tripQuoteUrl = $"{tripQuoteUrl}/{modelQuoteRequest.ReturnDate}";
            }

            return $"{tripQuoteUrl}?sort=bestflight_a";
        }

        /// <summary>
        ///  Search for the flight price store in the html element 'price-text'
        /// </summary>
        /// <param name="searchResult"></param>
        /// <returns>the flight price is found or 'Unknown'</returns>
        public static string GetFlightCost(IElement searchResult)
        {
            try
            {
                return searchResult.GetElementsByClassName("price-text")?.First()?.InnerHtml;
            }
            catch
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// Get flight itineraries need to complete the trip.
        /// </summary>
        /// <param name="searchResult"></param>
        /// <returns>return list itinerary for flights in search result</returns>
        public static List<IFlightItinerary> GetFlightItineraries(IElement searchResult)
        {
            IHtmlCollection<IElement> flights = GetFlights(searchResult);

            //Itinerary for one-way or two-way trip 
            var flightItineraries = new List<IFlightItinerary>();
            foreach (var flight in flights)
            {
                string[] airports = GetAirports(flight);
                flightItineraries.Add(new FlightItinerary
                {
                    OutboundIataCode = airports[0],
                    InboundIataCode = airports[1],
                    AirlineName = GetCarrierName(flight),
                    NumberOfStops = GetNumberOfStops(flight).Trim()
                });
            }

            return flightItineraries;
        }

        #region private methods
        //The airline company flying.
        private static string GetCarrierName(IElement flight)
        {
            try
            {
                return flight.GetElementsByClassName("section times")?.First()
                    .GetElementsByClassName("bottom")?.First()?.InnerHtml?.Trim();
            }
            catch
            {
                return "Unkownn";
            }
        }

        //THe number of stops need to complete flight.
        private static string GetNumberOfStops(IElement flight)
        {
            try
            {
                string innerHtml = flight.GetElementsByClassName("section stops")?.First()?.FirstElementChild.InnerHtml;
                if (innerHtml.IndexOf("stops") > 0) return innerHtml.Substring(0, innerHtml.IndexOf("stops")) + " stops";
                if (innerHtml.IndexOf("stop") > 0) return innerHtml.Substring(0, innerHtml.IndexOf("stop")) + " stop";
            }
            catch
            {
                //log
            }

            return "Unknown";
        }


        //The best flights deals.
        private static IHtmlCollection<IElement> GetFlights(IElement searchResult)
        {
            var resultInner = searchResult.QuerySelector(".resultInner");
            return resultInner.GetElementsByClassName("flights")?.FirstOrDefault()?.GetElementsByClassName("flight");
        }

        //The IATA airport or city code.
        private static string[] GetAirports(IElement flight)
        {
            try
            {
                return flight.QuerySelectorAll(".section.duration .bottom-airport")
                    .Select(_ => _.QuerySelector("span").InnerHtml.Trim()).ToArray();
            }
            catch
            {
                return new[] { "", "" };
            }
        }
        #endregion
    }
}