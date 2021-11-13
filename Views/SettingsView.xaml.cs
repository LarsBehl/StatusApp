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
		private string _backendUrl;

		public string BackendUrl
        {
			get => this._backendUrl;
			set
            {
				this._backendUrl = value;
				OnPropertyChanged(nameof(this.BackendUrl));
            }
        }

		public SettingsView()
		{
			InitializeComponent();
			this.BindingContext = this;
			this._settingsService = MauiProgram.App.Services.GetRequiredService<IAppsettingsService>();
			this.BackendUrl = this._settingsService.GetBackendUrl();
		}

		void SetBackendUrl(object sender, EventArgs e)
        {
			this.UrlInput.Unfocus();
			this._settingsService.StoreBackendUrl(this.UrlInput.Text.Trim());
        }

		void ClearSettings(object sender, EventArgs e)
        {
			this._settingsService.ClearBackendUrl();
        }
	}
}