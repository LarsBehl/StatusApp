using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusApp.Domain.Model.DTOs
{
    public class UserCreationRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }

        public UserCreationRequest(string username, string password, string token)
        {
            this.Username = username;
            this.Password = password;
            this.Token = token;
        }
    }
}
