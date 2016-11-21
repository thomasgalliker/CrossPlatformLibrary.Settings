using System.Reflection;

using CrossPlatformLibrary.Settings.IntegrationTests;

using Xunit.Runners.UI;

namespace IntegrationTests.UWP
{
    /// <summary>
    ///     Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : RunnerApplication
    {
        protected override void OnInitializeRunner()
        {
            this.AddTestAssembly(typeof(SettingsServiceTests).GetTypeInfo().Assembly);
        }
    }
}