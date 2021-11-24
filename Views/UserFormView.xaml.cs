using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using StatusApp.Domain.Model.DTOs;
using StatusApp.Services;

namespace StatusApp.Views
{
    public partial class UserFormView : ContentPage
    {
        private static readonly string ERROR_MSG = "There was an error creating the user";

        private readonly IUserService _userService;

        private bool _isLoading;
        private bool _hasCreationError;
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

        public bool HasCreationError
        {
            get => this._hasCreationError;
            set
            {
                this._hasCreationError = value;
                this.OnPropertyChanged(nameof(this.HasCreationError));
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

        public Func<string, bool> ValidateInput { get => (value) => string.IsNullOrWhiteSpace(value); }

        public Func<string, bool> ValidatePasswordConfirm { get => (value) => value == this.PasswordInput.InputText; }

        public UserFormView()
        {
            InitializeComponent();
            this.BindingContext = this;
            this._userService = MauiProgram.App.Services.GetRequiredService<IUserService>();
        }

        async void OnBackClicked(object sender, EventArgs e)
        {
            this.UsernameInput.Clear();
            this.PasswordInput.Clear();
            this.PasswordConfirmInput.Clear();
            this.TokenInput.Clear();

            await this.Navigation.PopAsync();
        }

        async void OnSubmitClicked(object sender, EventArgs e)
        {
            if (this.UsernameInput.InputHasError
                || this.PasswordInput.InputHasError
                || this.PasswordConfirmInput.InputHasError
                || this.TokenInput.InputHasError)
                return;

            this.IsLoading = true;

            UserResponse user = await this._userService.CreateUserAsync(this.UsernameInput.InputContent, this.PasswordInput.InputContent, this.TokenInput.InputContent);

            if(user is null)
            {
                this.ErrorMessage = ERROR_MSG;
                this.HasCreationError = true;
            }

            this.IsLoading = false;

            if (user is not null)
                await this.Navigation.PopAsync();
        }
    }
}