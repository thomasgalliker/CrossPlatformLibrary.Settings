namespace CrossPlatformLibrary.Settings
{
    public class SettingsService : ISettingsService
    {
        public T GetValueOrDefault<T>(string key, T defaultValue = default(T))
        {
            throw new NotImplementedInReferenceAssemblyException();
        }

        public void AddOrUpdateValue<T>(string key, T value)
        {
            throw new NotImplementedInReferenceAssemblyException();
        }
    }
}