using Microsoft.Maui.Essentials;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
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

        public void RemoveToken()
        {
            SecureStorage.Remove(TOKEN_NAME);
        }

        public async Task StoreTokenAsync(string token)
        {
            await SecureStorage.SetAsync(TOKEN_NAME, token);
        }
    }
}
