using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class FeedbackRequestDTO
    {

        public string AccountId { get; set; }

        public string ProductId { get; set; }

        public string Comment { get; set; }

        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; }

        // Additional properties can be added as needed
    }
}
