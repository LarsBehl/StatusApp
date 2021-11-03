﻿using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using StatusApp.Components;
using StatusApp.Domain.Model;
using StatusApp.Extensions;
using StatusApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StatusApp
{
    public partial class MainPage : ContentPage
    {
        private readonly IServicesService _servicesService;
        private List<ServiceInformation> _services;
        private string _message;
        private bool _isLoading;
        private bool _noData;

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

        public MainPage(IServicesService servicesService)
        {
            InitializeComponent();
            BindingContext = this;
            this.RefreshView.Command = new Command(async () =>
            {
                await this.GetServices();
                this.FinishedLoading();
            });
            this._servicesService = servicesService;
            this.RefreshView.IsRefreshing = true;
            this.IsLoading = true;
            this.NoData = false;
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
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
            bool isFirst = true;
            foreach (ServiceInformation service in this._services)
            {
                if(!isFirst)
                {
                    this.ServiceList.Add(new BoxView()
                    {
                        HeightRequest = 2,
                        BackgroundColor = Colors.Gray
                    });
                }
                else
                    isFirst = false;
                this.ServiceList.Add(new ServiceComponent(service));
            }
        }

        private void FinishedLoading()
        {
            this.IsLoading = false;
            this.RefreshView.IsRefreshing = false;
        }
    }
}
