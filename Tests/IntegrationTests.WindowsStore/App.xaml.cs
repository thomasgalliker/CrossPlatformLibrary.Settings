using System.Reflection;
using CrossPlatformLibrary.Settings.IntegrationTests;

using Xunit.Runners.UI;

namespace IntegrationTests.WindowsStore
{
    public sealed partial class App : RunnerApplication
    {
        protected override void OnInitializeRunner()
        {
            this.AddTestAssembly(typeof(SettingsServiceTests).GetTypeInfo().Assembly);
        }
    }
}