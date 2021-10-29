using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using StatusApp.Services;
using System;
using Application = Microsoft.Maui.Controls.Application;

namespace StatusApp
{
    public partial class App : Application
	{
		protected static IServiceProvider ServiceProvider { get; set; }

		public App()
		{
			InitializeComponent();

			this.Setup();

			MainPage = new MainPage(ServiceProvider.GetRequiredService<IAppsettingsService>());
		}

        private void Setup()
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton<IAppsettingsService, AppsettingsService>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
