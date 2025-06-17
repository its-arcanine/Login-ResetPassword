using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Feedback
    {
        public string FeedbackId { get; set; } // Unique identifier for the feedback
        public string AccountId { get; set; } // Identifier for the customer who provided the feedback
        public string ProductId { get; set; } // Identifier for the product being reviewed
        public string Comment { get; set; } // Feedback comment provided by the customer
        public int Rating { get; set; } // Rating given by the customer (e.g., 1 to 5 stars)
        public DateTime CreatedAt { get; set; } // Timestamp when the feedback was created
 // Timestamp when the feedback was last updated
    }
}
