using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Dtos
{
    public class CartItemDto
    {
        public int ID { get; set; }

        public string ProductTitle { get; set; }
        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public double TotalPrice { get { return Price * Quantity; } }


        public double Price { get; set; }
    }
}
