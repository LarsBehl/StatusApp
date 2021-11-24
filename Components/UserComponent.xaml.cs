using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using StatusApp.Domain.Model.DTOs;

namespace StatusApp.Components
{
	public partial class UserComponent : ContentView
	{
		private UserResponse _user;

		public event EventHandler<int> OnDelete;

		public UserResponse User
        {
			get => this._user;
			set
            {
				this._user = value;
				this.OnPropertyChanged(nameof(this.User));
            }
        }

		public UserComponent()
		{
			InitializeComponent();
			this.BindingContext = this;
		}

		public UserComponent(UserResponse user) : this()
        {
			this.User = user;
        }

		public void OnDeleteClicked(object sender, EventArgs e)
        {
			this.OnDelete?.Invoke(this, this.User.Id);
        }
	}
}