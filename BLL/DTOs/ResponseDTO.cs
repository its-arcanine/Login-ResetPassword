using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{

        public class ResponseDTO
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
        }

        public class ResponseDTO<T> : ResponseDTO
        {   
            public T Data { get; set; }
        }
  
}
