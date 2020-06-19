using System;

namespace Flightscraper.Spa.Interfaces
{
    public interface ISkyscannerItinerary
    {
        int[] CarrierIds { get; set; }
        DateTime DepartureDate { get; set; }
        int DestinationId { get; set; }
        int OriginId { get; set; }
    }
}