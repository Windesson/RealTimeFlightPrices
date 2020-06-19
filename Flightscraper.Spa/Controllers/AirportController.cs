using System;
using System.Collections.Generic;
using System.Web.Http.Description;
using System.Net;
using System.Web.Http;
using Flightscraper.Spa.Interfaces;
using Flightscraper.Spa.Conf;
using Flightscraper.Spa.Models;
using Unity;
using log4net;

namespace Flightscraper.Spa.Controllers
{
    /// <summary>
    ///  Class to help user quickly find the desired airport.
    /// </summary>
    public class AirportController : ApiController
    {
        private readonly IAirportRepository _airportRepository;
        private static readonly ILog Logger;

        static AirportController()
        {
            Logger = UnityConfig.Container.Resolve<ILoggerWrapper>().GetLogger(typeof(AirportController));
        }

        /// <summary>
        ///  Load airport repository.
        /// </summary>
        public AirportController()
        {
            _airportRepository = UnityConfig.Container.Resolve<IAirportRepository>();
        }

        /// <summary>
        ///  Search aiport based on the query value.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [ResponseType(typeof(IEnumerable<Airport>))]
        public IHttpActionResult GetAirports(string query)
        {
            try
            {
                IEnumerable<Airport> result = query.ToLower() == "all" ? 
                    _airportRepository.GetAirports() : 
                    _airportRepository.FindAirports(query);

                return Ok(result);

            }
            catch (Exception ex)
            {
                Logger.Error($"{ex.Message}|{ex.InnerException}");
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
    }
}
