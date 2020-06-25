using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Core.Internal;
using Flightscraper.Spa.Conf;
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
        private static readonly IEnumerable<Airport> Airports;
        private static readonly ILog Logger;

        static AirportRepository()
        {
            Logger = UnityConfig.Container.Resolve<ILoggerWrapper>().GetLogger(typeof(FlightRepository));
            var filePath = System.Web.HttpContext.Current.Server.MapPath("~/OpenFlights/airports.dat");
            Airports = ReadOpenFlightAirports(filePath);
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
                        var city = values[2].Replace("\"", "");
                        var country = values[3].Replace("\"", "");

                        //we only want airport that contain the below information.
                        if (iataCode.Length != 3 || city.IsNullOrEmpty() || country.IsNullOrEmpty()) continue;

                        var airport = new Airport();
                        if (int.TryParse(values[0], out var id))
                        {
                            airport.Id = id;
                        }

                        airport.Name = values[1].Replace("\"","");
                        airport.City = city;
                        airport.Country = country;
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
        /// Find aiports on query 
        /// </summary>
        /// <param name="query">string query to search</param>
        /// <returns></returns>
        public IEnumerable<Airport> FindAirports(string query, int limit)
        {
            return Airports
                .Where(_ => _.City.StartsWith(query, StringComparison.OrdinalIgnoreCase) ||
                            _.IATA.StartsWith(query, StringComparison.OrdinalIgnoreCase) ||
                            _.Country.StartsWith(query, StringComparison.OrdinalIgnoreCase)) 
                .OrderByDescending(_ => _.Country == "United States").ThenBy(_ => _.City).Take(limit);
        }

        /// <summary>
        /// Get all 14110 airport in the airports.dat file orderby name
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Airport> GetAirports()
        {
            return Airports
                .OrderBy(_ => _.Name);
        }

    }
}