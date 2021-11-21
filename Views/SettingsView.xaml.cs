using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using StatusApp.Services;
using System;

namespace StatusApp.Views
{
    public partial class SettingsView : ContentPage
	{
		private readonly IAppsettingsService _settingsService;
		private readonly IServiceInformationService _servicesService;
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
			this._servicesService = MauiProgram.App.Services.GetRequiredService<IServiceInformationService>();
			this._userService = MauiProgram.App.Services.GetRequiredService<IUserService>();
			this._userService.OnAutomaticLogout += async (sender, args) =>
			{
				this.IsLoggedIn = false;
				await this.Navigation.PopToRootAsync();
				await DisplayAlert("Logout", "You have been logged out due to Token expiration", "OK");

			};
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

		async void NavigateServicesView(object sender, EventArgs e)
        {
			await this.Navigation.PushAsync(new ServicesView());
			Console.WriteLine("Navigating to Services view");
        }

		void ClearSettings(object sender, EventArgs e)
        {
			this._settingsService.ClearBackendUrl();
			this.BackendUrl = string.Empty;
        }
	}
}