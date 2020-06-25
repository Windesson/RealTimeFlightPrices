using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly ILog Logger;

        static AirportRepository()
        {
            Logger = UnityConfig.Container.Resolve<ILoggerWrapper>().GetLogger(typeof(FlightRepository));
        }

        /// <summary>
        /// Private method that returns a database context.
        /// </summary>
        /// <returns>An instance of the Context class.</returns>
        static Context GetContext()
        {
            var context = new Context();
            context.Database.Log = (message) => Logger.Info(message);
            return context;
        }


        /// <summary>
        /// Returns an enumerable of matching airports.
        /// </summary>
        /// <param name="query">string query to search</param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public IEnumerable<Airport> FindAirports(string query, int limit)
        {
            query = query.ToLower();
            IList<Airport> airports = new List<Airport>();
            try
           {
                using (Context context = GetContext())
                {
                    airports = context.Airports
                        .Where(_ => _.City.ToLower().StartsWith(query) || _.IATA.ToLower().StartsWith(query) || _.Country.ToLower().StartsWith(query))
                        .OrderByDescending(_ => _.Country == "United States").ThenBy(_ => _.City).Take(limit).ToList();
                }
            }
            catch (Exception error)
            {
                Logger.Error(error);
            }
        
            return airports;
        }

        /// <summary>
        /// Returns an enumerable of all airports.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Airport> GetAirports()
        {
            IList<Airport> airports = new List<Airport>();
            try
            {
                using (Context context = GetContext())
                {
                    airports = context.Airports
                        .OrderByDescending(_ => _.Country == "United States").ThenBy(_ => _.City).ToList();
                }
            }
            catch (Exception error)
            {
                Logger.Error(error);
            }

            return airports;
        }

    }
}