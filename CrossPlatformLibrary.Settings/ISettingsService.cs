
namespace CrossPlatformLibrary.Settings
{
    public interface ISettingsService
    {
        T GetValueOrDefault<T>(string key, T defaultValue = default(T));

        void AddOrUpdateValue<T>(string key, T value);
    }
}