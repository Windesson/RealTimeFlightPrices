using Flightscraper.Spa.Interfaces;

namespace Flightscraper.Spa.Models
{
    public class FlightRequest : IFlightRequest
    {
        public string Origin { get; set; }

        public string DepartDate { get; set; }

        public string ReturnDate { get; set; }

        public string Destination { get; set; }
    }
}
