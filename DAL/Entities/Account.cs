using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Account
    {
        public string AccountId { get; set; }
        public string AccountEmail { get; set; }
        public string ?AccountPassword { get; set; }
        
        public bool IsVerified { get; set; } = false; // Default to false, indicating the account is not verified
        public string ?AccountName { get; set; }

        public string ?Token { get; set; }
        public DateTime ?TokenExpiration { get; set; }
    }
}
