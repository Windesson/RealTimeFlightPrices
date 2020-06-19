using System.Collections.Generic;

namespace Flightscraper.Spa.Interfaces
{
    public interface IFlightResponse
    {
        int Id { get; }
        List<IFlightItinerary> FlightItineraries { get; }
        string TripTotalPrice { get; }
        string BookingLink { get; }
        string QuoteSource { get; }
    }
}