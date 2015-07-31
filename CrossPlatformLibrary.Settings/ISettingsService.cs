
namespace CrossPlatformLibrary.Settings
{
    public interface ISettingsService
    {
        T GetValueOrDefault<T>(string key, T defaultValue = default(T));

        bool AddOrUpdateValue<T>(string key, T value);
    }
}