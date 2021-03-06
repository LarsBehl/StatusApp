using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using StatusApp.Components;
using StatusApp.Domain.Model;
using StatusApp.Domain.Model.DTOs;
using StatusApp.Extensions;
using StatusApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StatusApp.Views
{
    public partial class HomeView : ContentPage
    {
        private readonly IServiceInformationService _servicesService;
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

        public HomeView()
        {
            InitializeComponent();
            BindingContext = this;
            this._servicesService = MauiProgram.App.Services.GetRequiredService<IServiceInformationService>();
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
            if (this.NoData)
                this.NoData = false;
            this._services = await this._servicesService.GetServiceInformation();
            if (this._services is null)
            {
                this.NoData = true;
                this.Message = "Service unavailable. Is the backend url configured?";
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
                this.ServiceList.Add(new ServiceInformationComponent(service));
            }
        }

        private void FinishedLoading()
        {
            this.IsLoading = false;
            this.ClickButton.IsEnabled = true;
        }
    }
}
