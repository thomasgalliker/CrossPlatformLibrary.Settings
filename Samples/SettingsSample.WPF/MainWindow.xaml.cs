using System;
using System.Linq;
using System.Windows;

using CrossPlatformLibrary.IoC;

using SettingsSample.Model;
using SettingsSample.Services;

namespace SettingsSample.WPF
{
    public partial class MainWindow : Window
    {
        private readonly IDemoSettingsService demoSettingsService;

        public MainWindow()
        {
            this.InitializeComponent();

            // TODO GATH: Harmonize this code with the xamarin forms project
            // TODO GATH: Install .Settings nuget

            this.demoSettingsService = SimpleIoc.Default.GetInstance<IDemoSettingsService>();
            this.demoSettingsService.Id = Guid.NewGuid();
            var guid = this.demoSettingsService.Id;
            this.demoSettingsService.DecimalValue += this.demoSettingsService.DecimalValue;
            this.demoSettingsService.FloatValue += this.demoSettingsService.FloatValue;
            ////this.demoSettingsService.UshortNullable = 1; // TODO GATH: Nullable<?> doesnt work yet!
            ////this.demoSettingsService.UshortNullable += this.demoSettingsService.UshortNullable;

            var personModels = this.demoSettingsService.PersonModels.ToList();
            personModels.Add(new PersonModel { Name = "Person1", Age = 1 });
            personModels.Add(new PersonModel { Name = "Person2", Age = 2 });
            this.demoSettingsService.PersonModels = personModels;

            this.demoSettingsService.LastLoaded += new TimeSpan(1, 0, 0, 0);
            this.demoSettingsService.LastLoaded += new TimeSpan(1, 0, 0, 0);
            this.demoSettingsService.LastLoaded += new TimeSpan(1, 0, 0, 0);

            this.Button.Click += this.OnButtonClick;

            this.RefreshNumberOfClicks();
        }

        void OnButtonClick(object sender, RoutedEventArgs e)
        {
            this.demoSettingsService.NumberOfClicks++;
            this.RefreshNumberOfClicks();
        }

        private void RefreshNumberOfClicks()
        {
            this.Button.Content = string.Format("IDemoSettingsService.NumberOfClicks={0}", this.demoSettingsService.NumberOfClicks);
        }
    }
}