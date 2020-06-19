using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flightscraper.Spa.Conf;
using Flightscraper.Spa.Extension;
using Flightscraper.Spa.Interfaces;
using Flightscraper.Spa.Models;
using log4net;
using Unity;

namespace Flightscraper.Spa.Data
{
    /// <summary>
    /// A class to search airport data using iata, city, name or country
    /// </summary>
    public class AirportRepository : IAirportRepository
    {
        private readonly IEnumerable<Airport> _airports;
        private static readonly ILog Logger;

        static AirportRepository()
        {
            Logger = UnityConfig.Container.Resolve<ILoggerWrapper>().GetLogger(typeof(FlightRepository));
        }

        /// <summary>
        ///  Load airport data stored locally.
        ///  Data file provied by openflights.org
        /// </summary>
        public AirportRepository()
        {
            var filePath = System.Web.HttpContext.Current.Server.MapPath("~/OpenFlights/airports.dat");
            _airports = ReadOpenFlightAirports(filePath);
        }

        /// <summary>
        /// Load airport data to memory 
        /// </summary>
        /// <param name="filePath">file path to openflights airport data file</param>
        /// <returns></returns>
        public static IEnumerable<Airport> ReadOpenFlightAirports(string filePath)
        {
            var airportResults = new List<Airport>();

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string line = "";
                    reader.ReadLine();
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');

                        var iataCode = values[4].Replace("\"", "");
                        if(iataCode.Length != 3) continue;

                        var airport = new Airport();
                        if (int.TryParse(values[0], out var id))
                        {
                            airport.AirportId = id;
                        }

                        airport.Name = values[1].Replace("\"","");
                        airport.City = values[2].Replace("\"", "");
                        airport.Country = values[3].Replace("\"", "");
                        airport.IATA = iataCode;
                        airport.ICAO = values[5].Replace("\"", "");

                        airportResults.Add(airport);
                    }
                }

            }
            catch(Exception ex)
            {
                Logger.Error($"{ex.Message}|{ex.InnerException}");
            }

            return airportResults;
        }

        /// <summary>
        /// Find aiports on query matching either the City, Name, IATA or Country
        /// </summary>
        /// <param name="query">string query to search</param>
        /// <returns></returns>
        public IEnumerable<Airport> FindAirports(string query)
        {
            return _airports
                .Where(_ => _.City.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            _.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            _.IATA.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            _.Country.Contains(query, StringComparison.OrdinalIgnoreCase)) 
                .OrderBy(_ => _.Name);
        }

        /// <summary>
        /// Get all 14110 airport in the airports.dat file orderby name
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Airport> GetAirports()
        {
            return _airports
                .OrderBy(_ => _.Name);
        }

    }
}