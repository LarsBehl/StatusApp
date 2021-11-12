using Microsoft.Extensions.DependencyInjection;
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
        private bool _noData = false;

        public bool IsLoading
        {
            get => this._isLoading;
            set
            {
                this._isLoading = value;
                OnPropertyChanged(nameof(this.IsLoading));
            }
        }

        public bool NoData
        {
            get => this._noData;
            set
            {
                this._noData = value;
                OnPropertyChanged(nameof(this.NoData));
            }
        }

        public string Message
        {
            get => this._message;
            set
            {
                this._message = value;
                OnPropertyChanged(nameof(this.Message));
            }
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            this._servicesService = MauiProgram.App.Services.GetRequiredService<IServicesService>();
            this.ClickButton.IsEnabled = false;
            this.GetServices().GetAwaiter().OnCompleted(this.FinishedLoading);
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            this.ClickButton.IsEnabled = false;
            this.IsLoading = true;
            await this.GetServices();
            this.FinishedLoading();
        }

        private async Task GetServices()
        {
            Console.WriteLine("Called Service");
            if (this.NoData)
                this.NoData = false;
            this._services = await this._servicesService.GetServiceInformation();
            if (this._services is null)
            {
                this.NoData = true;
                this.Message = "Service unavailable";
                return;
            }

            if (this._services.IsEmtpy())
            {
                this.NoData = false;
                this.Message = "No Services configured";
                return;
            }
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
