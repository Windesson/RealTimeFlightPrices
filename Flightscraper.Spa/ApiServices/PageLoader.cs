using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Flightscraper.Spa.Data;

namespace Flightscraper.Spa.ApiServices
{
    /// <summary>
    ///  Load Html page using AngleSharp api
    /// </summary>
    internal class PageLoader : IPageLoader
    {
        private static readonly HttpClient HttpClientService;

        /// <summary>
        ///  Create unique http client
        /// </summary>
        static PageLoader()
        {
            HttpClientService = new HttpClient();
        }

        /// <summary>
        ///  Fetch a html web page as HTMLDocument
        /// </summary>
        /// <param name="siteUrl">An URL to a web page</param>
        /// <returns></returns>
        public async Task<IHtmlDocument> FetchHtmlDocument(string siteUrl)
        {
            using (HttpResponseMessage request = await HttpClientService.GetAsync(siteUrl))
            {
                if (request.StatusCode != System.Net.HttpStatusCode.OK) throw new Exception($"return statusCode: {request.StatusCode}");

                using (Stream response = await request.Content.ReadAsStreamAsync())
                {
                    var parser = new HtmlParser();
                    return parser.ParseDocument(response);

                }
            }

        }
    }
}
