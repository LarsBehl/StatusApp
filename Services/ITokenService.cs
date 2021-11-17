using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusApp.Services
{
    public interface ITokenService
    {
        Task StoreTokenAsync(string token);
        Task<string> LoadTokenAsync();
    }
}
