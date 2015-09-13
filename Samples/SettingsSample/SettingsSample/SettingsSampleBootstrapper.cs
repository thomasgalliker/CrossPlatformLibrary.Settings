
using CrossPlatformLibrary.Bootstrapping;
using CrossPlatformLibrary.IoC;

using SettingsSample.Services;

namespace SettingsSample
{
    public class SettingsSampleBootstrapper : Bootstrapper
    {
        protected override void ConfigureContainer(ISimpleIoc container)
        {
            container.Register<IDemoSettingsService, DemoSettingsService>();
        }
    }
}
