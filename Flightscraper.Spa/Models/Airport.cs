using System.ComponentModel.DataAnnotations;

namespace Flightscraper.Spa.Models
{
    public class Airport
    {
        //Unique OpenFlights identifier for this airport.
        public int Id { get; set; }

        //Main city served by airport. 
        public string City { get; set; }

        //Country or territory where airport is located
        public string Country { get; set; }

        //3-letter International Air Transport Association airport code.
        [MaxLength(3)]
        public string IATA { get; set; }

        //This is shown by the autocomplete in the UI form.
        public string Label => Country == "United States" ? $"{City} ({IATA})" : $"{City}, {Country} ({IATA})";

    }
}