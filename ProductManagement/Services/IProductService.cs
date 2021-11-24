using ProductManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();

    }
}
