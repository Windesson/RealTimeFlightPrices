using System.Threading.Tasks;
using Flightscraper.Spa.Models;

namespace Flightscraper.Spa.Interfaces
{
    public interface ISkyscannerLoader
    {
        string GetBaseBookingUrl();
        Task<SkyscannerBrowseQuoteResponse> BrowseQuotesAsync(IFlightRequest modelQuoteRequest);
    }
}