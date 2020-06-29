using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flightscraper.Spa.Conf;
using Flightscraper.Spa.Interfaces;
using Flightscraper.Spa.Models;
using log4net;
using Unity;

namespace Flightscraper.Spa.ApiServices
{
    /// <summary>
    ///  Read flights response from Skyscanner API and parse to a common flight response
    /// </summary>
    public class SkyscannerResponseReader : IFlightResponseReader
    {
        private static readonly ILog Logger;
        private static readonly ISkyscannerLoader Loader;

        static SkyscannerResponseReader()
        {
            Logger = UnityConfig.Container.Resolve<ILoggerWrapper>().GetLogger(typeof(SkyscannerResponseReader));
            Loader = UnityConfig.Container.Resolve<ISkyscannerLoader>();
        }

        /// <summary>
        /// Browse for flights quotes from skyscanner Api
        /// </summary>
        /// <param name="request">An user flight request</param>
        /// <returns></returns>
        public async Task<List<IFlightResponse>> BrowseQuotesAsync(IFlightRequest request)
        {
            var flightResponses = new List<IFlightResponse>();
            try
            {
                //load flights from skyscanner api
                SkyscannerBrowseQuoteResponse response = await Loader.BrowseQuotesAsync(request);

                //parse flight response to the model shown to the user
                flightResponses.AddRange(response.Quotes.Select(quote => ParseResponse(request, response, quote)));
            }
            catch(Exception ex)
            {
                //log exception
                Logger.Error($"{ex.Message}|{ex.InnerException}");
            }

            //return flight response for one-was or round trip.
            return flightResponses;
        }

        #region private method

        private static IFlightResponse ParseResponse(IFlightRequest modelQuoteRequest, SkyscannerBrowseQuoteResponse response,
            Quote quote)
        {
            IFlightItinerary outboundItinerary = GetItinerary(response, quote.OutboundLeg);
            IFlightItinerary inboundItinerary = GetItinerary(response, quote.InboundLeg);

            var flightItineraries = new List<IFlightItinerary>();
            if (inboundItinerary != null) flightItineraries.Add(inboundItinerary);
            if (outboundItinerary != null) flightItineraries.Add(outboundItinerary);

            IFlightResponse aFlightResponse = new FlightResponse()
            {
                QuoteSource = "Skyscanner",
                TripTotalPrice = TryGetCost(quote.MinPrice) ?? quote.MinPrice,
                BookingLink = GetBookingLink(modelQuoteRequest),
                FlightItineraries = flightItineraries
            };

            return aFlightResponse;
        }

        private static IFlightItinerary GetItinerary(SkyscannerBrowseQuoteResponse skyscannerBrowseResponse, ISkyscannerItinerary itinerary)
        {
            if (itinerary == null) return null;
            var stops = itinerary.CarrierIds?.Length ?? 0;
            var flightItinerary = new FlightItinerary
            {
                NumberOfStops = GetFlightItineraryNumberOfStops(stops),
                OutboundIataCode = GetOutboundIataCode(skyscannerBrowseResponse, itinerary),
                InboundIataCode = GetInboundIataCode(skyscannerBrowseResponse, itinerary)
            };
            foreach (var carrier in skyscannerBrowseResponse.Carriers.Where(_ => itinerary.CarrierIds.Contains(_.CarrierId)))
            {
                flightItinerary.AirlineName += carrier.Name;
            }

            return flightItinerary;
        }

        private static string GetInboundIataCode(SkyscannerBrowseQuoteResponse skyscannerBrowseResponse, ISkyscannerItinerary itinerary)
        {
            return skyscannerBrowseResponse.Places.First(_ => _.PlaceId == itinerary.DestinationId).IataCode;
        }

        private static string GetOutboundIataCode(SkyscannerBrowseQuoteResponse skyscannerBrowseResponse, ISkyscannerItinerary itinerary)
        {
            return skyscannerBrowseResponse.Places.First(_ => _.PlaceId == itinerary.OriginId).IataCode;
        }

        private static string GetFlightItineraryNumberOfStops(int stops)
        {
            return stops == 0 ? "non stop" : stops > 1 ? $"{stops} stops" : $"{stops} stop";
        }

        private static string GetBookingLink(IFlightRequest modelQuoteRequest)
        {
            return $"{Loader.GetBaseBookingUrl()}/{modelQuoteRequest.Origin}/{modelQuoteRequest.Destination}" +
                $"/{modelQuoteRequest.DepartDate}/{modelQuoteRequest.ReturnDate}";
        }

        private static string TryGetCost(string minPrice)
        {
            try
            {
                return $"${(int)Convert.ToDouble(minPrice)}";
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
