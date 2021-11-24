using StatusApp.Domain;
using StatusApp.Domain.Model.DTOs;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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
        private CancellationTokenSource _logoutCts;
        private System.Timers.Timer _timer;
        private JwtSecurityTokenHandler _tokenHandler;
        private HttpClient _authorizedClient;

        public UserResponse CurrentUser { get; set; }

        public event EventHandler<EventArgs> OnAutomaticLogout;

        public UserService(IAppsettingsService appsettingsService, ITokenService tokenService)
        {
            this._appsettingsService = appsettingsService;
            this._tokenService = tokenService;
            this._timer = new System.Timers.Timer()
            {
                AutoReset = false
            };
            this._tokenHandler = new JwtSecurityTokenHandler();
            this._httpClient = null;
            this.CurrentUser = null;
            this._authorizedClient = null;
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
            HttpResponseMessage response;
            try
            {
                this._cts.CancelAfter(TimeSpan.FromSeconds(5));
                response = await this._httpClient.PostAsJsonAsync("/authenticate", new LoginRequest(username, password), this._cts.Token);

                if (response.StatusCode != HttpStatusCode.OK)
                    return LoginResponseType.Fault;
            }
            catch (Exception)
            {
                return LoginResponseType.ServiceUnavailable;
            }

            LoginResponse loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

            await this._tokenService.StoreTokenAsync(loginResponse.Token);
            this.CurrentUser = loginResponse.User;
            // decode the token
            JwtSecurityToken token = this._tokenHandler.ReadJwtToken(loginResponse.Token);
            // set timer to handle automatic logout
            this._logoutCts = new CancellationTokenSource();
            this._timer.Interval = (token.ValidTo - DateTime.UtcNow).TotalMilliseconds - 1000;
            this._timer.Elapsed += (sender, args) =>
            {
                if(!this._logoutCts.Token.IsCancellationRequested)
                {
                    this._logoutCts = null;
                    this.OnAutomaticLogout.Invoke(this, null);
                    this.LogoutUser();
                }
            };
            this._timer.Start();

            return LoginResponseType.Success;
        }

        public void LogoutUser()
        {
            if (this._logoutCts is not null)
            {
                this._logoutCts.Cancel();
                this._timer.Stop();
            }
            this._tokenService.RemoveToken();
            this.CurrentUser = null;
            this._authorizedClient = null;
        }

        public async Task<TokenCreationResponse> CreateUserCreationTokenAsync()
        {
            if(this._authorizedClient is null)
            {
                this._authorizedClient = new HttpClient();
                this._authorizedClient.BaseAddress = new Uri(this._appsettingsService.GetBackendUrl());
                this._authorizedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await this._tokenService.LoadTokenAsync());
            }

            HttpResponseMessage response;
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(5));
                response = await this._authorizedClient.PostAsync("/users/token", null, cts.Token);

                if (response.StatusCode != HttpStatusCode.Created)
                    return null;
            }
            catch(Exception)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<TokenCreationResponse>();
        }

        public async Task<UserResponse> CreateUserAsync(string username, string password, string token)
        {
            if (this._httpClient is null)
            {
                string baseAddress = this._appsettingsService.GetBackendUrl();

                if (string.IsNullOrEmpty(baseAddress))
                    return null;

                this._httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(baseAddress)
                };
            }

            UserCreationRequest request = new UserCreationRequest(username.Trim(), password.Trim(), token.Trim());

            HttpResponseMessage response;
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(5));
                response = await this._httpClient.PostAsJsonAsync<UserCreationRequest>("/users", request, cts.Token);

                if (response.StatusCode != HttpStatusCode.Created)
                    return null;
            }
            catch (Exception)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<UserResponse>();
        }
    }
}
