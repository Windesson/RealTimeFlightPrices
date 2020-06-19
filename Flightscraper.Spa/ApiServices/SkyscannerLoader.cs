using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flightscraper.Spa.Interfaces;
using Flightscraper.Spa.Models;
using Newtonsoft.Json;

namespace Flightscraper.Spa.ApiServices
{
    /// <summary>
    ///  A web client to fetch flights from rapid api skyscanner services
    /// </summary>
    internal class SkyscannerLoader : ISkyscannerLoader
    {
        private const string RapidApiHostAppSettingName = "x-rapidapi-host";
        private const string RapidApiKeyAppSettingName = "x-rapidapi-key";
        private static readonly string SecretRapidApiHost;
        private static readonly string SecretRapidApiKey;
        private static readonly HttpClient LocalHttpClient;
        private static readonly string BaseBookingUrl;
        private static readonly string ApiServiceUrl;

        /// <summary>
        ///  Load web client settings and secret key using ConfigurationManager 
        /// </summary>
        static SkyscannerLoader()
        {
            ApiServiceUrl = ConfigurationManager.AppSettings["Rapidapi:skyscanner:apiservices:url"];
            BaseBookingUrl = ConfigurationManager.AppSettings["Skyscanner:booking:flights"];
            SecretRapidApiHost = ConfigurationManager.AppSettings[RapidApiHostAppSettingName];
            SecretRapidApiKey = ConfigurationManager.AppSettings[RapidApiKeyAppSettingName];
            LocalHttpClient = new HttpClient();
        }

        /// <summary>
        ///  Access skyscanner base booking url
        /// </summary>
        /// <returns>book url defined in the app setting</returns>
        public string GetBaseBookingUrl()
        {
            return BaseBookingUrl;
        }

        /// <summary>
        ///  Perform the actual web api search for flights sync 
        /// </summary>
        /// <param name="modelQuoteRequest">model wrapping the actual user request</param>
        /// <returns></returns>
        public async Task<SkyscannerBrowseQuoteResponse> BrowseQuotesAsync(IFlightRequest modelQuoteRequest)
        {
            if(!ValidateApiSetting()) throw new Exception("Require skyscanner secret api host, key and url");
            
            using (var httpRequest = GetHttpRequestMessage(modelQuoteRequest))
            using (var responseMessage = await LocalHttpClient.SendAsync(httpRequest)) {
                if (!responseMessage.IsSuccessStatusCode)
                    throw new Exception($"Rapid Skyscanner-Flight search status code `{responseMessage.StatusCode}` Error:{responseMessage.ReasonPhrase}");
                using (var readStream = new StreamReader(await responseMessage.Content.ReadAsStreamAsync(), Encoding.UTF8))
                    return JsonConvert.DeserializeObject<SkyscannerBrowseQuoteResponse>(readStream.ReadToEnd());
            }
        }

        #region private method
        private static HttpRequestMessage GetHttpRequestMessage(IFlightRequest modelQuoteRequest)
        {
            var httpRequest = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
            };
            httpRequest.Headers.Add(RapidApiHostAppSettingName, SecretRapidApiHost);
            httpRequest.Headers.Add(RapidApiKeyAppSettingName, SecretRapidApiKey);
            httpRequest.RequestUri = ParseApiRequestUri(modelQuoteRequest);
            return httpRequest;
        }

        private static bool ValidateApiSetting()
        {
            return ApiServiceUrl != null || SecretRapidApiHost != null || SecretRapidApiKey != null;
        }

        private static Uri ParseApiRequestUri(IFlightRequest modelQuoteRequest)
        {
            var browseQuotesUrl = $"{ApiServiceUrl}/browsequotes/v1.0/US/USD/en-US";
            return new Uri($"{browseQuotesUrl}/{modelQuoteRequest.Origin}/{modelQuoteRequest.Destination}" +
                           $"/{modelQuoteRequest.DepartDate}?inboundpartialdate={modelQuoteRequest.ReturnDate}");
        }

        #endregion

    }


}