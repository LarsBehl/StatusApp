using StatusApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Threading;

namespace StatusApp.Services
{
    public class ServicesService : IServicesService
    {
        private readonly IAppsettingsService _appsettingsService;
        private HttpClient _httpClient;
        private CancellationTokenSource _cts;

        public ServicesService(IAppsettingsService appsettingsService)
        {
            this._appsettingsService = appsettingsService;

            this._httpClient = new HttpClient()
            {
                BaseAddress = new Uri(this._appsettingsService.GetBackendUrl())
            };
        }

        public async Task<List<Service>> GetServices()
        {
            List<Service> result;
            this._cts = new CancellationTokenSource();
            
            try
            {
                this._cts.CancelAfter(TimeSpan.FromSeconds(5));
                result = await this._httpClient.GetFromJsonAsync<List<Service>>("/services", this._cts.Token);
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }
    }
}
