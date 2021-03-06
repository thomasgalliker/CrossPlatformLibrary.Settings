﻿using CrossPlatformLibrary.Bootstrapping;

using Xamarin.Forms;

namespace SettingsSample.Forms
{
    public partial class App : Application
    {
        private readonly IBootstrapper bootstrapper;

        public App()
        {
            this.InitializeComponent();

            this.bootstrapper = new SettingsSampleBootstrapper();
            this.bootstrapper.Startup();

            this.MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}