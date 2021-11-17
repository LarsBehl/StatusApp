using Microsoft.Maui.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusApp.Services
{
    public class TokenService : ITokenService
    {
        private static readonly string TOKEN_NAME = "user_token";

        public async Task<string> LoadTokenAsync()
        {
            return await SecureStorage.GetAsync(TOKEN_NAME);
        }

        public async Task StoreTokenAsync(string token)
        {
            await SecureStorage.SetAsync(TOKEN_NAME, token);
        }
    }
}
