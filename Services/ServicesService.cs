using StatusApp.Domain.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace StatusApp.Services
{
    public class ServicesService : IServicesService
    {
        private readonly ITokenService _tokenService;
        private readonly IAppsettingsService _appsettingsService;
        private HttpClient _httpClient;

        public ServicesService(ITokenService tokenService, IAppsettingsService appsettingsService)
        {
            this._tokenService = tokenService;
            this._appsettingsService = appsettingsService;
            this._httpClient = null;
        }

        public async Task<Service> CreateServiceAsync(string name, string url)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            HttpResponseMessage response;
            try
            {
                cts.CancelAfter(TimeSpan.FromSeconds(5));
                response = await _httpClient.PostAsJsonAsync("/services", new ServiceConfigurationRequest(name, url), cts.Token);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    return null;
            }
            catch (Exception)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Service>();
        }

        public async Task<List<Service>> GetServicesAsync()
        {
            if (this._httpClient is null)
            {
                this._httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(this._appsettingsService.GetBackendUrl())
                };

                this._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await this._tokenService.LoadTokenAsync());
            }

            List<Service> services = null;
            try
            {
                services = await this._httpClient.GetFromJsonAsync<List<Service>>("/services");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return services;
        }
    }
}
