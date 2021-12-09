using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;
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
            builder
                .UseSkiaSharp(true)
                .UseMauiApp<App>()
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
            services.AddSingleton<IServiceInformationService, ServiceInformationService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IServicesService, ServicesService>();
        }
    }
}