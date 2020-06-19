using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flightscraper.Spa.Interfaces
{
    public interface IFlightResponseReader
    {
        Task<List<IFlightResponse>> BrowseQuotesAsync(IFlightRequest request);
    }
}