using CrossPlatformLibrary.IoC;

using TypeConverter;

namespace CrossPlatformLibrary.Settings
{
    public class ContainerExtension : IContainerExtension
    {
        public void Initialize(ISimpleIoc container)
        {
            container.Register<IConverterRegistry, ConverterRegistry>("settingsConverterRegistry", false);
            container.Register<ISettingsService, SettingsService>(new ResolvedParameter<IConverterRegistry>("settingsConverterRegistry"));
        }
    }
}
