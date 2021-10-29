using StatusApp.Domain.Model;
using StatusApp.Domain.Model.Local;
using System;
using System.Net.Http;

namespace StatusApp.Components
{
    public partial class ServiceComponent
    {
        private readonly HttpClient _httpClient;

        // properties
        private Service _service;
        public Service Service
        {
            get => _service;
            set
            {
                this._service = value;
                OnPropertyChanged(nameof(Service));
            }
        }

        private ServiceStatusInformation _serviceStatusInformation;
        public ServiceStatusInformation ServiceStatusInformation
        {
            get => this._serviceStatusInformation;
            set
            {
                this._serviceStatusInformation = value;
                OnPropertyChanged(nameof(ServiceStatusInformation));
            }
        }


        public ServiceComponent()
        {
            InitializeComponent();
            this.BindingContext = this;
        }

        public ServiceComponent(Service service) : this()
        {
            this.Service = service;
            this._httpClient = new HttpClient()
            {
                BaseAddress = new Uri(this.Service.Url)
            };

            this.GetServiceStatus();
        }

        private void GetServiceStatus()
        {
            TimeOnly startTime = TimeOnly.FromDateTime(DateTime.Now);
            HttpResponseMessage response = this._httpClient.GetAsync(this.Service.Url).GetAwaiter().GetResult();
            TimeSpan elapsedTime = TimeOnly.FromDateTime(DateTime.Now) - startTime;
            this.ServiceStatusInformation = new ServiceStatusInformation()
            {
                StatusCode = response.StatusCode,
                ResponseTime = elapsedTime.TotalMilliseconds
            };
        }
    }
}