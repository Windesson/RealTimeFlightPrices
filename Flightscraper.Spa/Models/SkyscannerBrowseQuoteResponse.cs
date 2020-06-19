using Flightscraper.Spa.Interfaces;
using System;

namespace Flightscraper.Spa.Models
{
    public class SkyscannerBrowseQuoteResponse 
    {
        public Quote[] Quotes { get; set; }
        public Place[] Places { get; set; }
        public Carrier[] Carriers { get; set; }
        public Currency[] Currencies { get; set; }
    }

    public class Quote
    {
        public int QuoteId { get; set; }
        public string MinPrice { get; set; }
        public bool Direct { get; set; }
        public Outboundleg OutboundLeg { get; set; }
        public Inboundleg InboundLeg { get; set; }
        public DateTime QuoteDateTime { get; set; }
    }

    public class Outboundleg : ISkyscannerItinerary
    {
        public int[] CarrierIds { get; set; }
        public int OriginId { get; set; }
        public int DestinationId { get; set; }
        public DateTime DepartureDate { get; set; }
    }

    public class Inboundleg: ISkyscannerItinerary
    {
        public int[] CarrierIds { get; set; }
        public int OriginId { get; set; }
        public int DestinationId { get; set; }
        public DateTime DepartureDate { get; set; }
    }

    public class Place
    {
        public int PlaceId { get; set; }
        public string IataCode { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string SkyscannerCode { get; set; }

        public string CityName { get; set; }
        public string CityId { get; set; }
        public string CountryName { get; set; }
    }

    public class Carrier
    {
        public int CarrierId { get; set; }
        public string Name { get; set; }
    }

    public class Currency
    {
        public string Code { get; set; }
        public string Symbol { get; set; }
        public string ThousandsSeparator { get; set; }
        public string DecimalSeparator { get; set; }
        public bool SymbolOnLeft { get; set; }
        public bool SpaceBetweenAmountAndSymbol { get; set; }
        public int RoundingCoefficient { get; set; }
        public int DecimalDigits { get; set; }
    }

}
