using Microsoft.Maui.Essentials;
using StatusApp.Domain;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace StatusApp.Services
{
    public class AppsettingsService : IAppsettingsService
    {
        private static readonly string BACKEND_URL = "backend_url";
        private readonly Appsettings _appsettings;

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

        public void ClearBackendUrl()
        {
            Preferences.Remove(BACKEND_URL);
        }

        public string GetBackendUrl()
        {
            return Preferences.Get(BACKEND_URL, string.Empty);
        }

        public bool StoreBackendUrl(string url)
        {
            if(string.IsNullOrWhiteSpace(url))
                return false;

            Preferences.Set(BACKEND_URL, url);

            return true;
        }
    }
}
