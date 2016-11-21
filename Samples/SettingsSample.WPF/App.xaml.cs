using System.Windows;

using CrossPlatformLibrary.Bootstrapping;

namespace SettingsSample.WPF
{
    public partial class App : Application
    {
        private readonly IBootstrapper bootstrapper;

        protected App()
        {
            this.bootstrapper = new SettingsSampleBootstrapper();
            this.bootstrapper.Startup();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            this.bootstrapper.Startup();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            this.bootstrapper.Shutdown();
            base.OnExit(e);
        }
    }
}