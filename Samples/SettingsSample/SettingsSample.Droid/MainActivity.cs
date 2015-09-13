using System;

using Android;
using Android.App;
using Android.OS;
using Android.Widget;

using CrossPlatformLibrary.Bootstrapping;
using CrossPlatformLibrary.IoC;

using SettingsSample.Model;
using SettingsSample.Services;
using System.Linq;

namespace SettingsSample.Android
{
    [Activity(Label = "SettingsSample.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private int count = 1;
        private IDemoSettingsService demoSettingsService;
        private Button button;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            this.SetContentView(Resource.Layout.Main);

            var bootstrapper = new SettingsSampleBootstrapper();
            bootstrapper.Startup();

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

            // Get our button from the layout resource,
            // and attach an event to it
            this.button = this.FindViewById<Button>(Resource.Id.myButton);
            this.button.Click += delegate
                {
                    this.demoSettingsService.NumberOfClicks++;
                    this.RefreshNumberOfClicks();
                };

            this.RefreshNumberOfClicks();
        }

        private void RefreshNumberOfClicks()
        {
            this.button.Text = string.Format("IDemoSettingsService.NumberOfClicks={0}", this.demoSettingsService.NumberOfClicks);
        }
    }
}