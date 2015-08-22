using System.IO.IsolatedStorage;

using CrossPlatformLibrary.Tracing;
using CrossPlatformLibrary.Utils;

namespace CrossPlatformLibrary.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly object locker = new object();
        private readonly ITracer tracer;

        public SettingsService(ITracer tracer)
        {
            Guard.ArgumentNotNull(() => tracer);

            this.tracer = tracer;
        }

        private static IsolatedStorageSettings IsoSettings
        {
            get
            {
                return IsolatedStorageSettings.ApplicationSettings;
            }
        }

        public T GetValueOrDefault<T>(string key, T defaultValue = default(T))
        {
            Guard.ArgumentNotNullOrEmpty(() => key);

            this.tracer.Debug("GetValueOrDefault with key={0} of type {1}", key, typeof(T));

            T value;
            lock (this.locker)
            {
                // If the key exists, retrieve the value.
                if (IsoSettings.Contains(key))
                {
                    var tempValue = IsoSettings[key];
                    if (tempValue != null)
                    {
                        value = (T)tempValue;
                    }
                    else
                    {
                        value = defaultValue;
                    }
                }
                // Otherwise, use the default value.
                else
                {
                    value = defaultValue;
                }
            }

            return null != value ? value : defaultValue;
        }

        public void AddOrUpdateValue<T>(string key, T value)
        {
            Guard.ArgumentNotNullOrEmpty(() => key);

            this.tracer.Debug("AddOrUpdateValue with key={0} of type {1}", key, typeof(T));

            lock (this.locker)
            {
                // If the key exists
                if (IsoSettings.Contains(key))
                {
                    // If the value has changed
                    if (IsoSettings[key] != value as object)
                    {
                        // Store key new value
                        IsoSettings[key] = value;
                    }
                }
                // Otherwise create the key.
                else
                {
                    IsoSettings.Add(key, value);
                }

                IsoSettings.Save();
            }
        }
    }
}