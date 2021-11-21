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
	public partial class TokenCreationComponent : ContentPage
	{
		private readonly IUserService _userService;
		private string _tokenString;
		private string _expirationString;

		public string TokenString
        {
			get => this._tokenString;
			set
            {
				this._tokenString = value;
				this.OnPropertyChanged(nameof(this.TokenString));
            }
        }

		public string ExpirationString
        {
			get => this._expirationString;
			set
            {
				this._expirationString = value;
				this.OnPropertyChanged(nameof(this.ExpirationString));
            }
        }

		public TokenCreationComponent()
		{
			InitializeComponent();
			this.BindingContext = this;
			this.TokenString = "A7BD56F8A23B9E88";
			this.ExpirationString = "Sunday, November 28, 2021";
			this._userService = MauiProgram.App.Services.GetRequiredService<IUserService>();
			this.GetUserCreationToken().GetAwaiter().OnCompleted(() => { });
		}

		private async Task GetUserCreationToken()
        {
			TokenCreationResponse token = await this._userService.CreateUserCreationTokenAsync();
			this.TokenString = token.Token;
			this.ExpirationString = token.ExpiresAt.ToLongDateString();
        }

		public async void OnOkClicked(object sender, EventArgs e) => await this.Navigation.PopModalAsync();
	}
}