using Microsoft.Maui.Controls;
using StatusApp.Components;
using StatusApp.Domain.Model;
using StatusApp.Extensions;
using StatusApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StatusApp
{
    public partial class MainPage : ContentPage
    {
        private readonly IServicesService _servicesService;
        private List<Service> _services;
        private string _message;

        public MainPage(IServicesService servicesService)
        {
            InitializeComponent();
            BindingContext = this;
            this._servicesService = servicesService;
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            this.ClickButton.IsEnabled = false;
            this.GetServices().GetAwaiter().OnCompleted(() => this.ClickButton.IsEnabled = true);
        }

        private async Task GetServices()
        {
            this._services = await this._servicesService.GetServices();
            if (this._services is null)
                this._message = "Service unavailable";

            if (this._services.IsEmtpy())
                this._message = "No Services configured";
            this.ServiceList.Clear();
            foreach (Service service in this._services)
            {
                this.ServiceList.Add(new ServiceComponent(service));
            }
        }
    }
}
