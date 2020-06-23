using Flightscraper.Spa.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Castle.Core.Internal;
using Flightscraper.Spa.Conf;
using Flightscraper.Spa.Interfaces;
using log4net;
using Unity;

namespace Flightscraper.Spa.Controllers
{
    public class FlightController : ApiController
    {
        private readonly IFlightRepository _flightRepository;
        private static readonly ILog Logger;
        const int maxTotalDaysRangeAllowed = 334;

        /// <summary>
        ///  Set unique logger.
        /// </summary>
        static FlightController()
        {
            Logger = UnityConfig.Container.Resolve<ILoggerWrapper>().GetLogger(typeof(FlightController));
        }

        /// <inheritdoc />
        /// <summary>
        ///  Load flight repository using dependency injection. 
        /// </summary>
        public FlightController()
        {
            _flightRepository = UnityConfig.Container.Resolve<IFlightRepository>();
        }

        /// <summary>
        ///  An API route to browse for one-way/round or flights in real-time.
        /// </summary>
        /// <param name="originPlace">The origin place</param>
        /// <param name="destinationPlace">The destination place</param>
        /// <param name="outboundDate">The depart date</param>
        /// <param name="inboundDate">The return date</param>
        /// <returns></returns>
        [ResponseType(typeof(IEnumerable<IFlightResponse>))]
        public IHttpActionResult GetFlights(string originPlace, string destinationPlace, string outboundDate, string inboundDate = null)
        {
            try
            {
                //encapsulate route parameters as model object.
                var request = GetRequestModel(originPlace, destinationPlace, outboundDate, inboundDate);
                
                //validate request and set model state 
                Validate(request);

                //request invalid return bad request
                if (!ModelState.IsValid) return BadRequest(ModelState);

                //browse all different flight services registered for the best flight deal
                IEnumerable<IFlightResponse> result = _flightRepository.BrowseQuotes(request);
                
                //return http response 200 with list of flights
                return Ok(result);
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex.Message}|{ex.InnerException}");
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        #region private methods

        private void Validate(IFlightRequest flightRequest)
        {
            const int locationCodeLength = 3; //Find out the 3-letter code of a location (airport, city) or identify which airport or city uses a particular code
            if (flightRequest.Origin.IsNullOrEmpty() || flightRequest.Origin.Length < locationCodeLength)
                ModelState.AddModelError("originPlace", "REQUIRED The origin place, minimum a 3-letter code of a location (airport, city)");

            if (flightRequest.Destination.IsNullOrEmpty() || flightRequest.Destination.Length < locationCodeLength)
                ModelState.AddModelError("destinationPlace", "REQUIRED The destination place, minimum a 3-letter code of a location (airport, city)");

            //make sure depart/return dates make sense
            ValidateDates(flightRequest.DepartDate, flightRequest.ReturnDate);

        }

        private static FlightRequest GetRequestModel(string originPlace, string destinationPlace, string outboundDate, string inboundDate = null)
        {
            return new FlightRequest()
            {
                Origin = originPlace,
                Destination = destinationPlace,
                DepartDate = outboundDate,
                ReturnDate = inboundDate
            };
        }

        private void ValidateDates(string departDateString, string returnDateString)
        {

            if (ConvertDate(departDateString, out var departDate))
            {

                if (departDate < DateTime.Today) // cannot depart from the past 
                {
                    ModelState.AddModelError("outboundDate", $"Depart date cannot be less than today. `{DateTime.Now.ToString("MM/dd/yyyy")}`");
                }

                if (IsDateWithinAllowedRange(departDate)) // depart cannot be more than 334 days 
                {
                    ModelState.AddModelError("outboundDate", $"Depart date cannot be more than than {maxTotalDaysRangeAllowed} days from today's date.");
                }

                if (!returnDateString.IsNullOrEmpty()) // if return date is provided
                {
                    if (ConvertDate(returnDateString, out var returnDate))
                    {
                        if (returnDate < departDate)
                        { // cannot return a date less than the depart date
                            ModelState.AddModelError("inboundDate", "Return date must be equal or greater than the Depart date.");
                        }

                        if (IsDateWithinAllowedRange(returnDate)) // depart cannot be more than 334 days 
                        {
                            ModelState.AddModelError("inboundDate", $"Return date cannot be more than {maxTotalDaysRangeAllowed} days from today's date.");
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("inboundDate", "Return date expected format “yyyy-mm-dd” or empty string for one-way flight.");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("outboundDate", "Depart date expected format “yyyy-mm-dd”.");
            }

        }

        private static bool IsDateWithinAllowedRange(DateTime departDate)
        {
            return (departDate - DateTime.Today).TotalDays > maxTotalDaysRangeAllowed;
        }

        private bool  ConvertDate(string dateString, out DateTime date)
        {
            if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", null, DateTimeStyles.None, out date))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
