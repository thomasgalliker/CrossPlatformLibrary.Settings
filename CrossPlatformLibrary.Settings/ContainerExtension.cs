using CrossPlatformLibrary.IoC;

namespace CrossPlatformLibrary.Settings
{
    public class ContainerExtension : IContainerExtension
    {
        public void Initialize(ISimpleIoc container)
        {
            container.RegisterWithConvention<ISettingsService>();
        }
    }
}
