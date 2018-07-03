using System;

using Tracing;

using TypeConverter;

#if __UNIFIED__
using Foundation;
#else
using MonoTouch.Foundation;
#endif

namespace CrossPlatformLibrary.Settings
{
    public class SettingsService : SettingsServiceBase
    {
        private static readonly object Locker = new object();

        public SettingsService(ITracer tracer, IConverterRegistry converterRegistry) 
            : base(tracer, converterRegistry)
        {
        }
        
        protected override object GetValueOrDefaultFunction<T>(string key, T defaultValue)
        {
            lock (Locker)
            {
                using (var defaults = NSUserDefaults.StandardUserDefaults)
                {
                    var settingsValue = defaults.StringForKey(key);
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
                using (var defaults = NSUserDefaults.StandardUserDefaults)
                {
                    defaults.SetString(Convert.ToString(value), key);
                    defaults.Synchronize();
                }
            }
        }
        
        public void Remove(string key)
        {
            ////lock (this.locker)
            ////{
            ////    this.defaults.RemoveObject(key);
            ////    this.defaults.Synchronize();
            ////}
        }
    }
}