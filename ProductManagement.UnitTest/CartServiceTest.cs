using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using ProductManagement.Data;
using ProductManagement.Dtos;
using ProductManagement.Services;
using ProductManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ProductManagement.UnitTest
{
    public class CartServiceTest
    {
    
        [Fact]
        public async Task When_Calling_CartService_AddProduct_ProductAdded()
        {


            
            //Arrange

            Moq.Mock<ILogger<ProductService>> loggerMock = new Moq.Mock<ILogger<ProductService>>();
            Mock<IProductService> productSerivce = new Mock<IProductService>();
             Mock<ICallExternalSystemService> callExternalSystemService = new Mock<ICallExternalSystemService>();
            var options = new DbContextOptionsBuilder<ProductManagementDbContext>();
            options.UseInMemoryDatabase("ProductManagementDB");
            ProductManagementDbContext dbContext = new ProductManagementDbContext(options.Options);
            CartService cartService = new CartService(dbContext, productSerivce.Object, callExternalSystemService.Object, null);
            Product product = new Product { id = 1, title = "Mens Jeans", price = 10 };
            
            //Act

            cartService.AddProductToCart(product);
            var orderItem = await dbContext.OrderItems.SingleOrDefaultAsync(o => o.ProductID == product.id);

            //Assert
            Assert.NotNull(orderItem);




        }

        [Fact]
        public async Task When_Calling_CartService_AddProduct_CheckProductExist()
        {



            //Arrange

            Moq.Mock<ILogger<ProductService>> loggerMock = new Moq.Mock<ILogger<ProductService>>();
            Mock<IProductService> productSerivce = new Mock<IProductService>();
            Mock<ICallExternalSystemService> callExternalSystemService = new Mock<ICallExternalSystemService>();
            var options = new DbContextOptionsBuilder<ProductManagementDbContext>();
            options.UseInMemoryDatabase("ProductManagementDB");
            ProductManagementDbContext dbContext = new ProductManagementDbContext(options.Options);
            CartService cartService = new CartService(dbContext, productSerivce.Object, callExternalSystemService.Object, null);
            Product product = new Product { id = 1, title = "Mens Jeans", price = 10 };

            //Act

            cartService.AddProductToCart(product);
            cartService.AddProductToCart(product);
            var orderItem = await dbContext.OrderItems.SingleOrDefaultAsync(o => o.ProductID == product.id);

            //Assert
            Assert.True(orderItem.Quantity == 2);
            




        }

        [Fact]
        public async Task When_Calling_CartService_ConfirmOrder_True()
        {



            //Arrange
             Moq.Mock<ILogger<ProductService>> loggerMock = new Moq.Mock<ILogger<ProductService>>();
            Mock<IProductService> productSerivce = new Mock<IProductService>();
            Mock<ICallExternalSystemService> callExternalSystemService = new Mock<ICallExternalSystemService>();
            var options = new DbContextOptionsBuilder<ProductManagementDbContext>();
            options.UseInMemoryDatabase("ProductManagementDB");
            ProductManagementDbContext dbContext = new ProductManagementDbContext(options.Options);
            CartService cartService = new CartService(dbContext, productSerivce.Object, callExternalSystemService.Object, null);

            Product product = new Product { id = 1, title = "Mens Jeans", price = 10 };
            productSerivce.Setup(s => s.GetAllProductsAsync()).ReturnsAsync(new List<Product>() { product });

            callExternalSystemService.Setup(s => s.SubmitOrderToExternalSystem(It.IsAny<string>())).ReturnsAsync(true);

            //Act

            cartService.AddProductToCart(product);
            cartService.AddProductToCart(product);
            await cartService.ConfirmOrder();
            var orderItem = await dbContext.OrderItems.Where(o => o.OrderItemStatus == true).ToListAsync();

            //Assert
             Assert.True(orderItem.Count()==1);





        }
    }
}
