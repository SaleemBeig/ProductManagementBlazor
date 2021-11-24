using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using ProductManagement.Data;
using ProductManagement.Services;
using ProductManagement.Utilities;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ProductManagement.UnitTest
{
    public class ProductServiceTest
    {
    
        [Fact]
        public void When_Calling_ProductService_Expect_ListOfProducts()
        {
            
            //Arrange
            Moq.Mock<IHttpClientFactory> httpClientFactoryMock = new Moq.Mock<IHttpClientFactory>();
            Moq.Mock<IOptions<ExternalServices>> optionsMock = new Moq.Mock<IOptions<ExternalServices>>();
            Moq.Mock <ILogger<ProductService >> loggerMock = new Moq.Mock<ILogger<ProductService>>();
            Moq.Mock<ProductManagementDbContext> productManagementDbContext = new Moq.Mock<ProductManagementDbContext>();
            Mock<HttpResponseMessage> HttpResponseMessage = new Moq.Mock<HttpResponseMessage>();


             optionsMock.Setup(o => o.Value).Returns(new ExternalServices());
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var client = new HttpClient(mockHttpMessageHandler.Object);
            httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

          
            mockHttpMessageHandler.Protected()
              .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
              {
                  StatusCode = HttpStatusCode.OK,
                  Content = new StringContent("{'id':1,'title':'Shirt','price':'12'}"),
              });

            var options = new DbContextOptionsBuilder<ProductManagementDbContext>();
            options.UseInMemoryDatabase("ProductManagementDB");
   
             ProductManagementDbContext dbContext = new ProductManagementDbContext(options.Options);

           IProductService productService = new ProductService(dbContext, httpClientFactoryMock.Object,  optionsMock.Object, loggerMock.Object);

            //Act

          var products=  productService.GetAllProductsAsync();

            //Assert
            Assert.NotNull(products);

        }
    }
}
