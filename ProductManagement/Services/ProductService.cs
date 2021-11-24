using ProductManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using ProductManagement.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace ProductManagement.Services
{
    public class ProductService : IProductService
    {
        //private IEnumerable<Product> products = Array.Empty<Product>();
        private IHttpClientFactory _clientFactory;
        
        private IMemoryCache _memoryCache;
        private IOptions<ExternalServices> _options;
        private ILogger<ProductService> _logger;


        ProductManagementDbContext _productdbContext;
      
        public ProductService(ProductManagementDbContext dbcontext, IHttpClientFactory clientFactory,IOptions<ExternalServices> options, ILogger<ProductService> logger)
        {
            _clientFactory = clientFactory;
            
            //_memoryCache = memoryCache;
            _options = options;
            _logger = logger;

            _productdbContext = dbcontext;

        }
        /// <summary>
        /// Get Product Data by using In-Memory Db...
        /// </summary>
        /// <returns></returns>
        public async Task<List<Product>> GetAllProductsAsync()
        {

            var request = new HttpRequestMessage(HttpMethod.Get, _options.Value.ProductDataProvider);
            request.Headers.Add("Accept", "application/vnd.github.v3+json");
            request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            var client = _clientFactory.CreateClient();

            try
            {

                var products = await _productdbContext.Products.ToListAsync();

                if (products == null||products.Count==0)
                {
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        using var responseStream = await response.Content.ReadAsStreamAsync();
                        products = await JsonSerializer.DeserializeAsync<List<Product>>(responseStream);

                        _productdbContext.AddRange(products);
                        _productdbContext.SaveChanges();
                  
                    }
                    else
                    {
                        products = new List<Product>();
                    }
                }

                return products;



            }
            catch (AggregateException exc)
            {
                _logger.LogError(exc.Message);
                throw;
            }

        }

        /// <summary>
        /// Get Products Data by using Cache..
        /// </summary>
        /// <returns></returns>
        public async Task<List<Product>> GetAllProductsCacheAsync()
        {

            var request = new HttpRequestMessage(HttpMethod.Get, _options.Value.ProductDataProvider);
            request.Headers.Add("Accept", "application/vnd.github.v3+json");
            request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            var client = _clientFactory.CreateClient();

            try
            {

                var products = await _memoryCache.GetOrCreateAsync("Products", async p =>
                {
                    p.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(50000);

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        using var responseStream = await response.Content.ReadAsStreamAsync();
                        return await JsonSerializer.DeserializeAsync<List<Product>>(responseStream);
                    }
                    return new List<Product>();
                });

                return products;



            }
            catch (AggregateException exc)
            {
                _logger.LogError(exc.Message);
                throw;
            }

        }
    }
}
