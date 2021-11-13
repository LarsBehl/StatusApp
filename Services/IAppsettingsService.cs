namespace StatusApp.Services
{
    public interface IAppsettingsService
    {
        string GetBackendUrl();

        bool StoreBackendUrl(string url);

        void ClearBackendUrl();
    }
}
