using StatusApp.Domain.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusApp.Exceptions
{
    public class ErrorResponseException : Exception
    {
        public ErrorResponse Error { get; set; }

        public ErrorResponseException(ErrorResponse error) => this.Error = error;
    }
}
