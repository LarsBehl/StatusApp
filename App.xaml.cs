using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
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
            // hardcode light theme until styling is adjusted to work with dark theme
            App.Current.UserAppTheme = OSAppTheme.Light;

            MainPage = new SideNav();
        }
    }
}
