using System;
using System.Net.Http;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;
using StatusApp.Services;

namespace StatusApp
{
    public partial class MainPage : ContentPage
    {
        private IAppsettingsService _appsettingsService;
        int count = 0;
        private HttpClient _httpCLient = new HttpClient();

        public MainPage(IAppsettingsService appsettingsService)
        {
            InitializeComponent();
            this._appsettingsService = appsettingsService;
            this._httpCLient.BaseAddress = new Uri(this._appsettingsService.GetBackendUrl());
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;
            CounterLabel.Text = $"Current count: {count}";

            SemanticScreenReader.Announce(CounterLabel.Text);
            HttpResponseMessage response = null;
            try
            {
                response = this._httpCLient.GetAsync("/services").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine(response.Content);
        }
    }
}
