using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Flightscraper.Spa.Models
{
    public class Airport
    {
        public string Label => Country == "United States" ? $"{City} ({IATA})" : $"{City}, {Country} ({IATA})";

        //Unique OpenFlights identifier for this airport.
        public int AirportId { get; set; }

        //Name of airport. May or may not contain the City name.
        public string Name { get; set; }

        //Main city served by airport. May be spelled differently from Name.
        public string City { get; set; }

        //Country or territory where airport is located
        public string Country { get; set; }

        //3-letter IATA code. Null if not assigned/unknown.
        [MaxLength(3)]
        public string IATA { get; set; }

        //4-letter ICAO code. Null if not assigned.
        [MaxLength(4)]
        public string ICAO { get; set; }


    }
}