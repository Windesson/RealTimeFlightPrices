using System.Collections.Generic;
using Flightscraper.Spa.Conf;
using Flightscraper.Spa.Interfaces;
using Unity;

namespace Flightscraper.Spa.Models
{
    public class FlightResponse : IFlightResponse
    {
        public FlightResponse()
        {
            Id = UnityConfig.Container.Resolve<IQuoteIdGenerator>().GetId();
        }

        public int Id { get; }

        public List<IFlightItinerary> FlightItineraries { get; internal set; }

        public string TripTotalPrice { get; internal set; }

        public string BookingLink { get; internal set; }

        public string QuoteSource { get; internal set; }
    }
}
