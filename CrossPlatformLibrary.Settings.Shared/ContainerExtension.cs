using CrossPlatformLibrary.IoC;

using TypeConverter;

namespace CrossPlatformLibrary.Settings
{
    public class ContainerExtension : IContainerExtension
    {
        public void Initialize(IIocContainer container)
        {
            container.RegisterSingleton<IConverterRegistry, ConverterRegistry>();
            container.RegisterSingleton<ISettingsService, SettingsService>();
        }
    }
}
