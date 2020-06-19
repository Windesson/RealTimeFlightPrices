using System.Threading.Tasks;
using AngleSharp.Html.Dom;

namespace Flightscraper.Spa.Data
{
    public interface IPageLoader
    {
        Task<IHtmlDocument> FetchHtmlDocument(string siteUrl);
    }
}