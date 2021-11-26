using StatusApp.Domain.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusApp.Exceptions
{
    public class WrongPasswordException : ErrorResponseException
    {
        public WrongPasswordException(ErrorResponse error) : base(error)
        {

        }
    }
}
