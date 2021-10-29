using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using StatusApp.Services;

namespace StatusApp
{
    public static class MauiProgram
    {
        public static MauiApp App { get; set; }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.Host.ConfigureServices(ConfigureServices);
            builder.UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });
            App = builder.Build();

            return App;
        }

        private static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
            services.AddSingleton<IAppsettingsService, AppsettingsService>();
            services.AddSingleton<IServicesService, ServicesService>();
        }
    }
}