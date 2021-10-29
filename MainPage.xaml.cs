using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;
using StatusApp.Domain.Model;
using StatusApp.Extensions;
using StatusApp.Services;

namespace StatusApp
{
    public partial class MainPage : ContentPage
    {
        private readonly IServicesService _servicesService;
        private readonly HttpClient _httpClient;
        private List<Service> _services;
        private string _labelValue = string.Empty;
        public string LabelValue
        {
            get => this._labelValue;
            set
            {
                this._labelValue = value;
                OnPropertyChanged(nameof(this.LabelValue));
            }
        }

        public MainPage(IServicesService servicesService)
        {
            InitializeComponent();
            BindingContext = this;
            this._servicesService = servicesService;
            this._httpClient = new HttpClient();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            this.ClickButton.IsEnabled = false;
            this.GetServices().GetAwaiter().OnCompleted(() => this.ClickButton.IsEnabled = true);
        }

        private async Task GetServices()
        {
            this._services = await this._servicesService.GetServices();
            LabelValue = this.GetServiceStatus();
        }

        private string GetServiceStatus()
        {
            string result = string.Empty;

            if (this._services is null)
                return "Service unavailable";

            if (this._services.IsEmtpy())
                return "No Services configured";

            foreach (Service service in this._services)
            {
                HttpResponseMessage response = this._httpClient.GetAsync(service.Url).GetAwaiter().GetResult();
                result += $"{service.Name}: Status {response.StatusCode}{Environment.NewLine}";
            }

            return result;
        }
    }
}
