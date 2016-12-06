using System;

using CrossPlatformLibrary.Callouts;
using CrossPlatformLibrary.IoC;

using SettingsSample.Services;

using Xamarin.Forms;

namespace SettingsSample.Forms
{
    public partial class MainPage : ContentPage
    {
        private readonly IDemoSettingsService demoSettingsService;
        private readonly ICallout callout;

        public MainPage()
        {
            this.InitializeComponent();

            this.demoSettingsService = SimpleIoc.Default.GetInstance<IDemoSettingsService>();
            this.callout = SimpleIoc.Default.GetInstance<ICallout>();
        }

        private void OnButtonAddSettingsClicked(object sender, EventArgs e)
        {
            this.demoSettingsService.NumberOfClicks++;
        }

        private void OnButtonGetSettingsClicked(object sender, EventArgs e)
        {
            this.callout.Show("NumberOfClicks", $"NumberOfClicks={this.demoSettingsService.NumberOfClicks}");
        }

        private void OnButtonResetSettingsClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}