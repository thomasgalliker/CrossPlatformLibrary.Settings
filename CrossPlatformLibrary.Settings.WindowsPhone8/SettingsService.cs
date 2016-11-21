using System.IO.IsolatedStorage;

using Tracing;

using TypeConverter;

namespace CrossPlatformLibrary.Settings
{
    public class SettingsService : SettingsServiceBase
    {
        private static readonly object Locker = new object();

        public SettingsService(ITracer tracer, IConverterRegistry converterRegistry)
            : base(tracer, converterRegistry)
        {
        }

        private static IsolatedStorageSettings IsoSettings
        {
            get
            {
                return IsolatedStorageSettings.ApplicationSettings;
            }
        }

        protected override object GetValueOrDefaultFunction<T>(string key, T defaultValue)
        {
            lock (Locker)
            {
                if (IsoSettings.Contains(key))
                {
                    var settingsValue = IsoSettings[key];
                    if (settingsValue != null)
                    {
                        return settingsValue;
                    }
                }
            }

            return defaultValue;
        }

        protected override void AddOrUpdateValueFunction<T>(string key, T value)
        {
            lock (Locker)
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