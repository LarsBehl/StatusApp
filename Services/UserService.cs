using Microsoft.Maui.Essentials;
using StatusApp.Domain;
using StatusApp.Domain.Model;
using StatusApp.Domain.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StatusApp.Services
{
    public class UserService : IUserService
    {
        private readonly IAppsettingsService _appsettingsService;
        private readonly ITokenService _tokenService;
        private CancellationTokenSource _cts;
        private HttpClient _httpClient;

        public UserResponse CurrentUser { get; set; }

        public UserService(IAppsettingsService appsettingsService, ITokenService tokenService)
        {
            this._appsettingsService = appsettingsService;
            this._tokenService = tokenService;
            this._httpClient = null;
            this.CurrentUser = null;
        }

        public async Task<LoginResponseType> LoginUserAsync(string username, string password)
        {
            if(this._httpClient is null)
            {
                string baseAddress = this._appsettingsService.GetBackendUrl();

                if (string.IsNullOrEmpty(baseAddress))
                    return LoginResponseType.ServiceUnavailable;

                this._httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(baseAddress)
                };
            }

            this._cts = new CancellationTokenSource();

            try
            {
                this._cts.CancelAfter(TimeSpan.FromSeconds(5));
                HttpResponseMessage response = await this._httpClient.PostAsJsonAsync("/authenticate", new LoginRequest(username, password), this._cts.Token);

                if (response.StatusCode != HttpStatusCode.OK)
                    return LoginResponseType.Fault;

                LoginResponse loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                await this._tokenService.StoreTokenAsync(loginResponse.Token);
                this.CurrentUser = loginResponse.User;
            }
            catch (Exception e)
            {
                return LoginResponseType.ServiceUnavailable;
            }

            return LoginResponseType.Success;
        }
    }
}
