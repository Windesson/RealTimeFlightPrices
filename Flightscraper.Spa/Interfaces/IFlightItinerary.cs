namespace Flightscraper.Spa.Interfaces
{
    public interface IFlightItinerary
    {
        string OutboundIataCode { get; }
        string InboundIataCode { get; }
        string AirlineName { get; }
        string NumberOfStops { get; }
    }
}