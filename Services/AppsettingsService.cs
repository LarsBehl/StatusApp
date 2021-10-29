using StatusApp.Domain;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace StatusApp.Services
{
    public class AppsettingsService : IAppsettingsService
    {
        private Appsettings _appsettings;

        public AppsettingsService()
        {
#if RELEASE
            string appsettingsName = "appsettings.json";
#elif DEBUG
            string appsettingsName = "appsettings.development.json";
#endif
            Stream appSettingsStream = Assembly.GetAssembly(typeof(Appsettings)).GetManifestResourceStream($"StatusApp.Resources.Configuration.{appsettingsName}");

            using StreamReader streamReader = new StreamReader(appSettingsStream);
            string jsonString = streamReader.ReadToEnd();
            this._appsettings = JsonSerializer.Deserialize<Appsettings>(jsonString);
        }

        public string GetBackendUrl()
        {
            return this._appsettings.BackendUrl;
        }
    }
}
