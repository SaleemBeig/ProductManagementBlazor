using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.Extensions.Logging;


namespace ProductManagement.Services
{
    public class CartService : ICartService
    {
        ProductManagementDbContext _productdbContext;
        IProductService _productService;
        ILogger _logger;
        ICallExternalSystemService _callExternalSystemService;

               
        public CartService(ProductManagementDbContext dbcontext, IProductService productService, ICallExternalSystemService callExternalSystemService, ILogger<CartService> logger)
        {
            _productdbContext = dbcontext;
            _productService = productService;
            _callExternalSystemService = callExternalSystemService;
            _logger = logger;


        }
        
        /// <summary>
        /// Add product to Cart..
        /// </summary>
        /// <param name="product"></param>
        public void AddProductToCart(Product product)
        {
            var orderitem=_productdbContext.OrderItems.SingleOrDefault(i => i.ProductID == product.id && i.OrderItemStatus==false);
            if (orderitem != null)
            {
                orderitem.Quantity += 1;

            }
            else
            {
                var orderItem = new OrderItem { ProductID = product.id, Quantity = 1, Price = product.price };
                _productdbContext.OrderItems.Add(orderItem);
                
            }
            _productdbContext.SaveChanges();



        }

        /// <summary>
        /// Confirm the Order to External system..
        /// </summary>
        /// <returns></returns>
        public async Task ConfirmOrder()
        {
            var orderItems = await _productdbContext.OrderItems.ToListAsync();
            var products = await _productService.GetAllProductsAsync();

            var result = from orderItem in orderItems
                         join product in products
                              on orderItem.ProductID equals product.id
                           select new ProductDto
                         {
                             id = product.id,
                             title = product.title,
                             price = product.price,
                             quantity = orderItem.Quantity
                         };

            var Order = new CartOrderDto
            {
                OrderStatus = "Confirmed",
                TotalPrice = result.Sum(orderItem => orderItem.price * orderItem.quantity),
                Products = result.ToList()
            };

           

            var orderJson =  System.Text.Json.JsonSerializer.Serialize<CartOrderDto>(Order);
            if( await _callExternalSystemService.SubmitOrderToExternalSystem(orderJson))
            {
                //Create Order...
                //Updte OrderItems with the newly created OrderId..
                orderItems.ForEach(item => { item.OrderItemStatus = true;});
                _productdbContext.OrderItems.UpdateRange(orderItems);
               await  _productdbContext.SaveChangesAsync();
            }


        }

        /// <summary>
        /// Get All Order Items for Cart..
        /// </summary>
        /// <returns></returns>
        public async Task<List<CartItemDto>> GetAllOrderItemsAsync()
        {

            try
            {
                var orderItems = await _productdbContext.OrderItems.ToListAsync();
                var products = await _productService.GetAllProductsAsync();

                //Get only Orderitem based on their status =false;
                var result = from orderItem in orderItems
                             join product in products
                            on orderItem.ProductID equals product.id
                            where orderItem.OrderItemStatus == false
                             select new CartItemDto
                             {
                                 ID = orderItem.ID,
                                 ProductID = orderItem.ProductID,
                                 ProductTitle = product.title,
                                 Price = product.price,
                                 Quantity = orderItem.Quantity
                             };

                return result.ToList();
            }catch(Exception exc)
            {
                _logger.LogError(exc.Message);
                throw;
            }

        }
        /// <summary>
        /// Remove Product from Cart...
        /// </summary>
        /// <param name="product"></param>
        public void RemoveProductFromCart(Product product)
        {
            var orderitem = _productdbContext.OrderItems.SingleOrDefault(i => i.ProductID == product.id);
            if (orderitem != null )
            {
                if (orderitem.Quantity > 1)
                {
                    orderitem.Quantity -= 1;
                }
                else
                {
                    _productdbContext.OrderItems.Remove(orderitem);
                }

            }
            _productdbContext.SaveChanges();
        }

    }
}
