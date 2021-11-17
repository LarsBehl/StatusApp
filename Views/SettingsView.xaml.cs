using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using StatusApp.Services;

namespace StatusApp.Views
{
	public partial class SettingsView : ContentPage
	{
		private readonly IAppsettingsService _settingsService;
		private readonly IServicesService _servicesService;
		private readonly IUserService _userService;
		private static readonly string ERROR_MSG = "The given url is not valid";
		private string _backendUrl;
		private bool _hasError;
		private bool _isLoggedIn;

		public string BackendUrl
        {
			get => this._backendUrl;
			set
            {
				this._backendUrl = value;
				OnPropertyChanged(nameof(this.BackendUrl));
            }
        }

		public bool HasError
        {
			get => this._hasError;
			set
            {
				this._hasError = value;
				OnPropertyChanged(nameof(this.HasError));
            }
        }

		public bool IsLoggedIn
        {
			get => this._isLoggedIn;
			set
            {
				this._isLoggedIn = value;
				this.OnPropertyChanged(nameof(this.IsLoggedIn));
            }
        }

		public SettingsView()
		{
			InitializeComponent();
			this.BindingContext = this;
			this._settingsService = MauiProgram.App.Services.GetRequiredService<IAppsettingsService>();
			this._servicesService = MauiProgram.App.Services.GetRequiredService<IServicesService>();
			this._userService = MauiProgram.App.Services.GetRequiredService<IUserService>();
			this.BackendUrl = this._settingsService.GetBackendUrl();
		}

		async void SetBackendUrl(object sender, EventArgs e)
        {
			this.HasError = false;
			this.UrlInput.Unfocus();
			Console.WriteLine("Unfocus called");
			this.UrlInput.IsEnabled = false;
			this.UrlInput.IsEnabled = true;

			if(!this._settingsService.StoreBackendUrl(this.UrlInput.Text.Trim()))
            {
				this.ErrorMessage.Text = ERROR_MSG;
				this.HasError = true;
			}


			if (await this._servicesService.GetServiceInformation() is null)
            {
				this.ErrorMessage.Text = ERROR_MSG;
				this.HasError = true;
			}
		}

		public void HandleLogin(object sender, EventArgs e)
        {
			Console.WriteLine("HandleLogin called");
			this.IsLoggedIn = true;
        }

		public void HandleLogout(object sender, EventArgs e)
        {
			Console.WriteLine("HandleLogout called");
			this._userService.LogoutUser();
			this.IsLoggedIn = false;
        }

		void UnfocusedEntry(object sender, EventArgs e)
        {
			Console.WriteLine("Handled unfocus event");
        }

		void ClearSettings(object sender, EventArgs e)
        {
			this._settingsService.ClearBackendUrl();
			this.BackendUrl = string.Empty;
        }
	}
}