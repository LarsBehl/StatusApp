using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using StatusApp.Services;
using System;
using Application = Microsoft.Maui.Controls.Application;

namespace StatusApp
{
    public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new MainPage(MauiProgram.App.Services.GetRequiredService<IServicesService>());
		}
    }
}
