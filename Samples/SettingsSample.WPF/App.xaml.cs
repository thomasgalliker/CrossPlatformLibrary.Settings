using System.Windows;

using CrossPlatformLibrary.Bootstrapping;

namespace SettingsSample.WPF
{
    public partial class App : Application
    {
        private readonly Bootstrapper bootstrapper;

        public App()
        {
            this.bootstrapper = new SettingsSampleBootstrapper();
            this.bootstrapper.Startup();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            this.bootstrapper.Shutdown();
            base.OnExit(e);
        }
    }
}