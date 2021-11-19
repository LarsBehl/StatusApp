using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using StatusApp.Components;
using StatusApp.Domain.Model.DTOs;
using StatusApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StatusApp.Views
{
    public partial class ServicesView : ContentPage
	{
		private static readonly string GENERIC_ERROR_MSG = "There was an error fetching the services";
		private static readonly string NO_SERVICES_MSG = "There are currently no services";

		private readonly IServicesService _servicesService;

		private bool _isLoading;
		private bool _hasLoadingError;
		private string _errorMessage;
		private List<Service> _services;

		public bool IsLoading
        {
			get => this._isLoading;
			set
            {
				this._isLoading = value;
				this.OnPropertyChanged(nameof(this.IsLoading));
            }
        }

		public bool HasLoadingError
        {
			get => this._hasLoadingError;
			set
            {
				this._hasLoadingError = value;
				this.OnPropertyChanged(nameof(this.HasLoadingError));
            }
        }

		public string ErrorMessage
        {
			get => this._errorMessage;
			set
            {
				this._errorMessage = value;
				this.OnPropertyChanged(nameof(this.ErrorMessage));
            }
        }

		public ServicesView()
		{
			InitializeComponent();
			this.BindingContext = this;
			this._servicesService = MauiProgram.App.Services.GetRequiredService<IServicesService>();

			this.LoadServices().GetAwaiter().OnCompleted(() => { });
		}

		private async Task LoadServices()
        {
			this.IsLoading = true;
			this.ServiceList.Clear();
			this._services = await this._servicesService.GetServicesAsync();

			if(this._services is null)
            {
				this.ErrorMessage = GENERIC_ERROR_MSG;
				this.HasLoadingError = true;
            }

			if(this._services.Count <= 0)
            {
				this.ErrorMessage = NO_SERVICES_MSG;
				this.HasLoadingError = true;
            }

			foreach (Service service in this._services)
				this.ServiceList.Add(new ServiceComponent(service));
			this.IsLoading = false;
        }

		public async void RefreshServices(object sender, EventArgs e) => await this.LoadServices();

		public async void CreateService(object sender, EventArgs e)
        {
			// TODO implement
			await this.Navigation.PushModalAsync(new ServiceCreationComponent());
        }
	}
}