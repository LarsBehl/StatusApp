using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusApp.Domain.Model.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public UserResponse User { get; set; }
    }
}
