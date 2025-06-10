using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class CartItemDTO
    {
        public string CartItemId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; } // Added for convenience, assuming you'll load Product details
        public int Quantity { get; set; }
        public decimal Price { get; set; } // Price of the item itself (quantity * unit price)
        public string ProductImageUrl { get; set; } // Added for convenience
    }
}
