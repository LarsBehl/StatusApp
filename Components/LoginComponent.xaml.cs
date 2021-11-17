using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using StatusApp.Domain;
using StatusApp.Services;

namespace StatusApp.Components
{
    public partial class LoginComponent : ContentView
    {
        private static readonly string SERVICE_UNAVAILABLE_MSG = "Service is unavailable";
        private static readonly string LOGIN_ERROR_MSG = "Invalid credentials";
        private readonly IUserService _userService;
        private bool _hasUsernameError;
        private bool _hasPasswordError;
        private bool _hasLoginError;
        private bool _isLoading;

        public event EventHandler<EventArgs> OnLoginSuccess;

        public bool HasUsernameError
        {
            get => this._hasUsernameError;
            set
            {
                this._hasUsernameError = value;
                this.OnPropertyChanged(nameof(this.HasUsernameError));
            }
        }

        public bool HasPasswordError
        {
            get => this._hasPasswordError;
            set
            {
                this._hasPasswordError = value;
                this.OnPropertyChanged(nameof(this.HasPasswordError));
            }
        }

        public bool HasLoginError
        {
            get => this._hasLoginError;
            set
            {
                this._hasLoginError = value;
                this.OnPropertyChanged(nameof(this.HasLoginError));
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

        public LoginComponent()
        {
            InitializeComponent();
            this.BindingContext = this;
            this._userService = MauiProgram.App.Services.GetRequiredService<IUserService>();
        }

        void FocusPassword(object sender, EventArgs e)
        {
            this.PasswordInput.Focus();
        }

        async void LoginUser(object sender, EventArgs e)
        {
            if (this.IsLoading)
                return;

            this.IsLoading = true;

            // TODO remove when Unfocus is implemented
            this.PasswordInput.IsEnabled = false;
            this.PasswordInput.IsEnabled = true;
            this.UsernameInput.IsEnabled = false;
            this.UsernameInput.IsEnabled = true;

            if (sender is Entry entry)
                entry.Unfocus();

            // reset current error states
            this.HasUsernameError = false;
            this.HasPasswordError = false;
            this.HasLoginError = false;

            if (!this.ValidateInputs())
            {
                this.IsLoading = false;
                return;
            }

            LoginResponseType responseType = await this._userService.LoginUserAsync(this.UsernameInput.Text, this.PasswordInput.Text);
            this.IsLoading = false;

            if (responseType == LoginResponseType.Success)
            {
                this.OnLoginSuccess.Invoke(this, null);
                return;
            }

            if (responseType == LoginResponseType.ServiceUnavailable)
                this.LoginError.Text = SERVICE_UNAVAILABLE_MSG;

            if (responseType == LoginResponseType.Fault)
                this.LoginError.Text = LOGIN_ERROR_MSG;

            this.HasLoginError = true;
        }

        public void UnfocusedEntry(object sender, EventArgs e) => this.ValidateInputs();

        private bool ValidateInputs()
        {
            this.HasLoginError = false;
            if (string.IsNullOrWhiteSpace(this.UsernameInput.Text))
            {
                this.HasUsernameError = true;
                return false;
            }

            this.HasUsernameError = false;

            if (string.IsNullOrWhiteSpace(this.PasswordInput.Text))
            {
                this.HasPasswordError = true;
                return false;
            }

            this.HasPasswordError = false;

            return true;
        }
    }
}