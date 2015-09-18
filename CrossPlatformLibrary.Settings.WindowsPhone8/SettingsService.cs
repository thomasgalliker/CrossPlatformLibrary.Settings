using System;
using System.IO.IsolatedStorage;

using CrossPlatformLibrary.Tracing;
using Guards;

using TypeConverter;

namespace CrossPlatformLibrary.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly object locker = new object();
        private readonly ITracer tracer;
        private readonly IConverterRegistry converterRegistry;

        public SettingsService(ITracer tracer, IConverterRegistry converterRegistry)
        {
            Guard.ArgumentNotNull(() => tracer);
            Guard.ArgumentNotNull(() => converterRegistry);

            this.tracer = tracer;
            this.converterRegistry = converterRegistry;
        }

        public IConverterRegistry ConverterRegistry
        {
            get
            {
                return this.converterRegistry;
            }
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
            Type targetType = typeof(T);
            this.tracer.Debug("GetValueOrDefault with key={0} of type {1}", key, targetType);

            T value = defaultValue;
            lock (this.locker)
            {
                if (IsoSettings.Contains(key))
                {
                    var settingsValue = IsoSettings[key];
                    if (settingsValue != null)
                    {
                        value = this.converterRegistry.TryConvert(settingsValue, defaultValue);
                    }
                }
            }

            return value;
        }

        public void AddOrUpdateValue<T>(string key, T value)
        {
            this.tracer.Debug("AddOrUpdateValue with key={0} of type {1}", key, typeof(T));

            lock (this.locker)
            {
                if (IsoSettings.Contains(key))
                {
                    if (IsoSettings[key] != value as object)
                    {
                        IsoSettings[key] = value;
                    }
                }
                else
                {
                    IsoSettings.Add(key, value);
                }

                IsoSettings.Save();
            }
        }
    }
}