using Xamarin.Utils;

namespace CrossPlatformLibrary.Settings
{
    public class IsolatedStorageProperty<T>
    {
        private readonly ISettingsService settingsService;
        private readonly string key;
        private readonly T defaultValue;

        public IsolatedStorageProperty(ISettingsService settingsService, string key, T defaultValue = default(T))
        {
            Guard.ArgumentNotNull(() => settingsService);
            Guard.ArgumentNotNullOrEmpty(() => key);

            this.settingsService = settingsService;
            this.key = key;
            this.defaultValue = defaultValue;
        }

        public T Value
        {
            get
            {
                return this.settingsService.GetValueOrDefault(this.key, this.defaultValue);
            }
            set
            {
                this.settingsService.AddOrUpdateValue(this.key, value);
            }
        }
    }
}