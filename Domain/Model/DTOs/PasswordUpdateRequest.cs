using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusApp.Domain.Model.DTOs
{
    public class PasswordUpdateRequest
    {
        public string OldPassword { get; set; }
        public string newPassword { get; set; }

        public PasswordUpdateRequest(string oldPassword, string newPassword)
        {
            this.OldPassword = oldPassword;
            this.newPassword = newPassword;
        }
    }
}
