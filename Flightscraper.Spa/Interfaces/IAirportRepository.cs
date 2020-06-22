using System.Collections.Generic;
using Flightscraper.Spa.Models;

namespace Flightscraper.Spa.Interfaces
{
    public interface IAirportRepository
    {
        IEnumerable<Airport> FindAirports(string query, int limit);
        IEnumerable<Airport> GetAirports();
    }
}