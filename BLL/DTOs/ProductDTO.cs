using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class ProductDTO
    {
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public string CategoryId { get; set; }
        public string ProductImageUrl { get; set; }
        public int ProductQuantity { get; set; }

        // Additional properties can be added as needed
    }
}
