using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace StatusApp.Views
{
    public partial class UserFormView : ContentPage
    {
        private bool _hasUsernameError;
        private bool _hasPasswordError;
        private bool _hasPasswordConfirmError;
        private bool _hasTokenError;
        private bool _isLoading;
        private bool _hasCreationError;
        private string _errorMessage;

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

        public bool HasPasswordConfirmError
        {
            get => this._hasPasswordConfirmError;
            set
            {
                this._hasPasswordConfirmError = value;
                this.OnPropertyChanged(nameof(this.HasPasswordConfirmError));
            }
        }

        public bool HasTokenError
        {
            get => this._hasTokenError;
            set
            {
                this._hasTokenError = value;
                this.OnPropertyChanged(nameof(this.HasTokenError));
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

        public string ErrorMessage
        {
            get => this._errorMessage;
            set
            {
                this._errorMessage = value;
                this.OnPropertyChanged(nameof(this.ErrorMessage));
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

        public UserFormComponent()
        {
            InitializeComponent();
            this.BindingContext = this;
        }

        async void OnBackClicked(object sender, EventArgs e)
        {
            // TODO
        }

        async void OnSubmitClicked(object sender, EventArgs e)
        { 
            // TODO
        }
    }
}