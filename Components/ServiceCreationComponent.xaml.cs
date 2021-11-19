using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using StatusApp.Domain.Model.DTOs;

namespace StatusApp.Components
{
	public partial class ServiceCreationComponent : ContentPage
	{
		private static readonly string SERVICE_CREATION_TITLE = "Create a new Service";
		private static readonly string SERVICE_EDIT_TITLE = "Edit an existing Service";

		private Service _service;
		private string _pageTitle;
		private bool _hasNameError;
		private bool _hasUrlError;

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

		public ServiceCreationComponent(): this(null)
		{
			
		}

		public ServiceCreationComponent(Service service)
        {
			InitializeComponent();
			this.BindingContext = this;
			this._service = service;

			if (this._service is not null)
			{
				this.PageTitle = SERVICE_EDIT_TITLE;
				this.NameInput.Text = this._service.Name;
				this.UrlInput.Text = this._service.Url;
			}
			else
				this.PageTitle = SERVICE_CREATION_TITLE;
		}

		private bool ValidateInput(string input) => !string.IsNullOrWhiteSpace(input);

		public async void OnBackButtonPressed(object sender, EventArgs e) => await this.Navigation.PopModalAsync();

		public async void OnSubmitButtonPressed(object sender, EventArgs e)
        {
			// TODO implement
			throw new NotImplementedException();
        }
	}
}