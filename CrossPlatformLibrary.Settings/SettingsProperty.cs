using System;

using CrossPlatformLibrary.Utils;

namespace CrossPlatformLibrary.Settings
{
    public class SettingsProperty<T>
    {
        private readonly ISettingsService settingsService;
        private readonly string key;
        private readonly T defaultValue;

        public SettingsProperty(ISettingsService settingsService, T defaultValue = default(T))
            : this(settingsService, Guid.NewGuid().ToString(), defaultValue)
        {
        }

        public SettingsProperty(ISettingsService settingsService, string key, T defaultValue = default(T))
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