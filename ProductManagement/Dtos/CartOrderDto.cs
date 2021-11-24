using ProductManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Dtos
{
    public class CartOrderDto
    {
        public string OrderStatus { get; set; }
        
        public double TotalPrice { get; set; }

        public List<ProductDto> Products { get; set; }
    }
}
