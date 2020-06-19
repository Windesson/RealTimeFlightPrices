using Flightscraper.Spa.Interfaces;

namespace Flightscraper.Spa.Models
{
    public class FlightItinerary : IFlightItinerary
    {
        public string OutboundIataCode { get; internal set; }

        public string InboundIataCode { get; internal set; }

        public string AirlineName { get; internal set; }

        public string NumberOfStops { get; internal set; }
    }
}