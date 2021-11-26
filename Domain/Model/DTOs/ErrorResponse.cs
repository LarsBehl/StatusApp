using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusApp.Domain.Model.DTOs
{
    public class ErrorResponse
    {
        public string Title { get; set; }
        public string DetailMessage { get; set; }
        public int ErrorCode { get; set; }
    }
}
