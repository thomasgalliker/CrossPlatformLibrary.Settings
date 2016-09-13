using System;

using CrossPlatformLibrary.IoC;

using SettingsSample.Services;

using Xamarin.Forms;

namespace SettingsSample.Forms
{
    public partial class MainPage : ContentPage
    {
        private readonly IDemoSettingsService demoSettingsService;

        public MainPage()
        {
            this.InitializeComponent();

            this.demoSettingsService = SimpleIoc.Default.GetInstance<IDemoSettingsService>();
        }

        private void OnButtonAddSettingsClicked(object sender, EventArgs e)
        {
            this.demoSettingsService.NumberOfClicks++;
        }

        private void OnButtonGetSettingsClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnButtonResetSettingsClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}