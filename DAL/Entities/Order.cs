using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Order
    {
        public string OrderId { get; set; } // Unique identifier for the order
        public string AccountId { get; set; } // Identifier for the customer who placed the order

        public string ProductId { get; set; } // Identifier for the product being ordered
        public int Quantity { get; set; } // Quantity of the product ordered

        public DateTime OrderDate { get; set; } // Date when the order was placed

        public decimal TotalAmount { get; set; } // Total amount for the order

        public string Address { get; set; } // Shipping address for the order

        public bool Status { get; set; } // Status of the order (e.g., Pending, Shipped, Delivered)
    }
}
