using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class CartItem
    {
        public string CartItemId { get; set; }
        public string CartId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Product Product { get; set; }
        public Cart Cart { get; set; }

    }
}
