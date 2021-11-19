using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using StatusApp.Domain.Model.DTOs;
using StatusApp.Services;

namespace StatusApp.Components
{
	public partial class ServiceCreationComponent : ContentPage
	{
		private static readonly string SERVICE_CREATION_TITLE = "Create a new Service";
		private static readonly string SERVICE_EDIT_TITLE = "Edit an existing Service";

		private readonly IServicesService _servicesService;

		private string _pageTitle;
		private bool _hasNameError;
		private bool _hasUrlError;
		private bool _hasConfigurationError;
		private bool _isLoading;

		public string PageTitle
        {
			get => this._pageTitle;
			set
            {
				this._pageTitle = value;
				this.OnPropertyChanged(nameof(this.PageTitle));
            }
        }

		public bool HasNameError
        {
			get => this._hasNameError;
			set
            {
				this._hasNameError = value;
				this.OnPropertyChanged(nameof(this.HasNameError));
            }
        }

		public bool HasUrlError
        {
			get => this._hasUrlError;
			set
            {
				this._hasUrlError = value;
				this.OnPropertyChanged(nameof(this.HasUrlError));
            }
        }

		public bool HasConfigurationError
        {
			get => this._hasConfigurationError;
			set
            {
				this._hasConfigurationError = value;
				this.OnPropertyChanged(nameof(this.HasConfigurationError));
            }
        }

		public bool IsLoading
        {
			get => this._isLoading;
			set
            {
				this._isLoading = value;
				this.OnPropertyChanged(nameof(this.IsLoading));
            }
        }

		public Service Service { get; set; }

		public ServiceCreationComponent(): this(null)
		{
			
		}

		public ServiceCreationComponent(Service service)
        {
			InitializeComponent();
			this.BindingContext = this;
			this.Service = service;
			this._servicesService = MauiProgram.App.Services.GetRequiredService<IServicesService>();

			if (this.Service is not null)
			{
				this.PageTitle = SERVICE_EDIT_TITLE;
				this.NameInput.Text = this.Service.Name;
				this.UrlInput.Text = this.Service.Url;
			}
			else
				this.PageTitle = SERVICE_CREATION_TITLE;
		}

		private bool ValidateInput(string input) => !string.IsNullOrWhiteSpace(input);

		public async void OnBackButtonPressed(object sender, EventArgs e)
		{
			this.Service = null;
			await this.Navigation.PopModalAsync();
		}

        protected override bool OnBackButtonPressed()
        {
			if (this.IsLoading)
				return true;
			this.Service = null;
			return base.OnBackButtonPressed();
        }

        public async void OnSubmitButtonPressed(object sender, EventArgs e)
        {
			this.IsLoading = true;
			this.HasUrlError = false;
			this.HasNameError = false;
			string url = this.UrlInput.Text;
			string name = this.NameInput.Text;

			if(!this.ValidateInput(name))
            {
				this.HasNameError = true;
				this.IsLoading = false;
				return;
            }

			bool success = Uri.TryCreate(url, UriKind.Absolute, out Uri _);

			if (!this.ValidateInput(url) || !success)
            {
				this.HasUrlError = true;
				this.IsLoading = false;
				return;
            }

			this.Service = await this._servicesService.CreateServiceAsync(name, url);

			if(this.Service is null)
				this.HasConfigurationError = true;

			this.IsLoading = false;
			await this.Navigation.PopModalAsync();
        }
	}
}