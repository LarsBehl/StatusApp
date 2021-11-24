using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using StatusApp.Components;
using StatusApp.Domain.Model.DTOs;
using StatusApp.Services;

namespace StatusApp.Views
{
	public partial class UsersView : ContentPage
	{
		private readonly string GENERIC_ERROR_MSG = "There was an issue requesting the users";
		private readonly string NO_USERS_MSG = "There are no users";

		private readonly IUserService _userService;

		private List<UserResponse> _users;
		private bool _isLoading;
		private bool _hasLoadingError;
		private string _errorMessage;

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

		public UsersView()
		{
			InitializeComponent();
			this.BindingContext = this;

			this._userService = MauiProgram.App.Services.GetRequiredService<IUserService>();

			this.LoadUsers().GetAwaiter().OnCompleted(() => { });
		}

		private async Task LoadUsers()
        {
			this.IsLoading = true;
			this._users = await this._userService.GetUsersAsync();

			if(this._users is null)
            {
				this.ErrorMessage = GENERIC_ERROR_MSG;
				this.HasLoadingError = true;
            }

			if(this._users?.Count <= 0)
            {
				this.ErrorMessage = NO_USERS_MSG;
				this.HasLoadingError = true;
            }

			foreach (UserResponse user in this._users)
            {
				UserComponent userComponent = new UserComponent(user);
				userComponent.OnDelete += this.DeletUser;
				this.UserList.Add(userComponent);
			}

			this.IsLoading = false;
        }

		public async void CreateUserToken(object sender, EventArgs e)
        {
			await this.Navigation.PushModalAsync(new TokenCreationComponent());
        }

		public async void RefreshUsers(object sender, EventArgs e)
        {
			this.UserList.Clear();
			await this.LoadUsers();
        }

		public async void DeletUser(object sender, int userId)
        {
			UserResponse user = this._users.Single(u => u.Id == userId);
			bool shouldDelete = await this.DisplayAlert("Delete user", $"Do you want to delete the user {user.Username}", "Yes", "No");

			if(shouldDelete)
            {
				bool success = await this._userService.DeleteUserAsync(userId);

				if (success)
                {
					if (user == this._userService.CurrentUser)
                    {
						this._userService.LogoutUser();
						return;
					}

					this.UserList.Clear();
					await this.LoadUsers();
                }
            }
        }
	}
}