using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using StatusApp;
using StatusApp.Exceptions;
using StatusApp.Services;

namespace StatusApp.Components
{
    public partial class UpdatePasswordComponent : ContentPage
    {
        private static readonly string PASSWORD_TOO_SHORT_MSG = "New password is too short";
        private static readonly string OLD_PASSWORD_WRONG_MSG = "Old password is invalid";
        private static readonly string GENERIC_ERROR_MSG = "There was an error communicating with the service";

        private readonly IUserService _userService;

        private bool _isLoading;
        private bool _hasUpdateError;
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

        public bool HasUpdateError
        {
            get => this._hasUpdateError;
            set
            {
                this._hasUpdateError = value;
                this.OnPropertyChanged(nameof(this.HasUpdateError));
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

        public Func<string, bool> ValidatePasswordConfirm { get => (value) => value == this.PasswordConfirmationInput.InputText; }

        public UpdatePasswordComponent()
        {
            InitializeComponent();
            this.BindingContext = this;
            this._userService = MauiProgram.App.Services.GetRequiredService<IUserService>();
        }

        protected override bool OnBackButtonPressed()
        {
            if (this.IsLoading)
                return true;

            this.ClearInputs();
            return base.OnBackButtonPressed();
        }

        async void OnBackButtonPressed(object sender, EventArgs e)
        {
            this.ClearInputs();
            await this.Navigation.PopModalAsync();
        }

        async void OnSubmitButtonPressed(object sender, EventArgs e)
        {
            this.HasUpdateError = false;
            this.IsLoading = true;

            if (this.OldPasswordInput.InputHasError
                || this.NewPasswordInput.InputHasError
                || this.PasswordConfirmationInput.InputHasError)
            {
                this.IsLoading = false;
                return;
            }

            try
            {
                bool success = await this._userService.UpdatePasswordAsync(this.OldPasswordInput.InputContent, this.NewPasswordInput.InputContent);

                if (!success)
                {
                    this.HasUpdateError = true;
                    this.ErrorMessage = GENERIC_ERROR_MSG;
                    return;
                }
            }
            catch (PasswordTooShortException)
            {
                Console.WriteLine("Password too short");
                this.HasUpdateError = true;
                this.ErrorMessage = PASSWORD_TOO_SHORT_MSG;
                return;
            }
            catch (WrongPasswordException)
            {
                Console.WriteLine("WrongPassword");
                this.HasUpdateError = true;
                this.ErrorMessage = OLD_PASSWORD_WRONG_MSG;
                return;
            }
            finally
            {
                this.IsLoading = false;
            }

            this.ClearInputs();
            await this.Navigation.PopModalAsync();
        }

        private void ClearInputs()
        {
            this.NewPasswordInput.Clear();
            this.OldPasswordInput.Clear();
            this.PasswordConfirmationInput.Clear();
        }
    }
}