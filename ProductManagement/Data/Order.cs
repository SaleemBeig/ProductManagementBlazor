using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Data
{
    public class Order
    {

        public int ID { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        public double TotalPrice
        {
            get
            {
                return OrderItems.Sum(s => s.TotalPrice);
            }


        }  
    }
}
