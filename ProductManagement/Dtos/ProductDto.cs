using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Dtos
{
    public class ProductDto
    {

        public int id { get; set; }

        public string title { get; set; }

        public double price { get; set; }

        public int quantity { get; set; }
    }
}
