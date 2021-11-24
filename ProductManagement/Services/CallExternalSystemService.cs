using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ProductManagement.Utilities;
using Microsoft.Extensions.Logging;
//using RestSharp;

namespace ProductManagement.Services
{
    public class CallExternalSystemService : ICallExternalSystemService
    {
        //private IEnumerable<Product> products = Array.Empty<Product>();
        private IHttpClientFactory _clientFactory;

        private IOptions<ExternalServices> _options;
        private ILogger _logger; 

        public CallExternalSystemService(IHttpClientFactory clientFactory, IOptions<ExternalServices> options, ILogger<CallExternalSystemService> logger )
        {
            _clientFactory = clientFactory;
            _options = options;
            _logger = logger;
        }

        /// <summary>
        /// The SubmitOrderToExternalSystem method has been created to submit the order details to the External Systems..
        /// </summary>
        /// <param name="jsonOrder"></param>
        /// <param name="systemURL"></param>
        /// <returns></returns>
        public async Task<bool> SubmitOrderToExternalSystem(string jsonOrder)
        {
           
                var request = new HttpRequestMessage(HttpMethod.Post, _options.Value.OrderConfirmationService);
                request.Headers.Add("Accept", "application/vnd.github.v3+json");
                request.Headers.Add("User-Agent", "HttpClientFactory-Sample");
                request.Content = new StringContent(jsonOrder, System.Text.Encoding.UTF8, "application/json");

                var client = _clientFactory.CreateClient();
                try
                {
                    var response = await client.SendAsync(request);

                _logger.LogInformation("Order submitted successfully.");

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                   
                        return false;
                   
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc.Message);
                    throw;

                }
            

          


        }
    }
}
