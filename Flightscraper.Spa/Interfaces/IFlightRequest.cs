namespace Flightscraper.Spa.Interfaces
{
    public interface IFlightRequest
    {
        string Origin { get; set; }
        string DepartDate { get; set; }
        string ReturnDate { get; set; }
        string Destination { get; set; }
    }
}