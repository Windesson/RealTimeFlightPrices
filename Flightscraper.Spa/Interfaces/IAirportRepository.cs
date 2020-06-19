using System.Collections.Generic;
using Flightscraper.Spa.Models;

namespace Flightscraper.Spa.Interfaces
{
    public interface IAirportRepository
    {
        IEnumerable<Airport> FindAirports(string query);
        IEnumerable<Airport> GetAirports();
    }
}