using StatusApp.Domain.Model;
using StatusApp.Domain.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace StatusApp.Services
{
    public class ServiceInformationService : IServiceInformationService
    {
        private readonly IAppsettingsService _appsettingsService;
        private HttpClient _httpClient;
        private CancellationTokenSource _cts;

        public ServiceInformationService(IAppsettingsService appsettingsService)
        {
            this._appsettingsService = appsettingsService;

            this._httpClient = null;
        }

        public async Task<List<ServiceInformation>> GetServiceInformation()
        {
            if(this._httpClient is null)
            {
                string url = this._appsettingsService.GetBackendUrl();

                if(string.IsNullOrEmpty(url))
                    return null;

                this._httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(url)
                };
            }

            List<ServiceInformation> result;
            this._cts = new CancellationTokenSource();
            
            try
            {
                this._cts.CancelAfter(TimeSpan.FromSeconds(5));
                result = await this._httpClient.GetFromJsonAsync<List<ServiceInformation>>("/services/information", this._cts.Token);
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }
    }
}
