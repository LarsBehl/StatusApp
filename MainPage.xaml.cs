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
        private List<ServiceInformation> _services;
        private string _message;
        private bool _isLoading = true;

        public bool IsLoading
        {
            get => this._isLoading;
            set
            {
                this._isLoading = value;
                OnPropertyChanged(nameof(this.IsLoading));
            }
        }

        public MainPage(IServicesService servicesService)
        {
            InitializeComponent();
            BindingContext = this;
            this._servicesService = servicesService;
            this.ClickButton.IsEnabled = false;
            this.GetServices().GetAwaiter().OnCompleted(this.FinishedLoading);
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            this.ClickButton.IsEnabled = false;
            this.IsLoading = true;
            this.GetServices().GetAwaiter().OnCompleted(this.FinishedLoading);
        }

        private async Task GetServices()
        {
            this._services = await this._servicesService.GetServiceInformation();
            if (this._services is null)
                this._message = "Service unavailable";

            if (this._services.IsEmtpy())
                this._message = "No Services configured";
            this.ServiceList.Clear();
            foreach (ServiceInformation service in this._services)
            {
                this.ServiceList.Add(new ServiceComponent(service));
            }
        }

        private void FinishedLoading()
        {
            this.IsLoading = false;
            this.ClickButton.IsEnabled = true;
        }
    }
}
