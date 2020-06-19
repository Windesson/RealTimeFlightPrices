using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flightscraper.Spa.Interfaces
{
    public interface IFlightRepository
    {
        IEnumerable<IFlightResponse> BrowseQuotes(IFlightRequest modelQuoteRequest);
    }
}