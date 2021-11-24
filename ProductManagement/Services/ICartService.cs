using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Data;
using ProductManagement.Dtos;

namespace ProductManagement.Services
{
   public interface ICartService
    {
        void AddProductToCart(Product product);
        void RemoveProductFromCart(Product product);
        Task<List<CartItemDto>>  GetAllOrderItemsAsync();
        Task ConfirmOrder();

    }
}
