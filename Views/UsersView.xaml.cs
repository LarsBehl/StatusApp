using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using StatusApp.Components;

namespace StatusApp.Views
{
	public partial class UsersView : ContentPage
	{
		public UsersView()
		{
			InitializeComponent();
		}

		public async void CreateUserToken(object sender, EventArgs e)
        {
			await this.Navigation.PushModalAsync(new TokenCreationComponent());
        }
	}
}