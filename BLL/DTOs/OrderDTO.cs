using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public  class OrderDTO
    {
        
        public string AccountId { get; set; }
        public string ProductId { get; set; }

        public DateTime OrderDate { get; set; }

        public int Quantity { get; set; }

        public decimal TotalAmount { get; set; }

        public string Address { get; set; }

 
        // Additional properties can be added as needed
    }
}
