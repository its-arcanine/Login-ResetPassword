using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class FeedbackDTO
    {
        public string Comment { get; set; }
        public int Rating { get; set; } // Assuming rating is an integer, e.g., 1 to 5
        public DateTime CreatedAt { get; set; }
    }
}
