using System;
using System.IO;
using System.IO.IsolatedStorage;
using Tracing;

using TypeConverter;

namespace CrossPlatformLibrary.Settings
{
    public class SettingsService : SettingsServiceBase
    {
        private readonly IsolatedStorageFile store;

        public SettingsService(ITracer tracer, IConverterRegistry converterRegistry) 
            : base(tracer, converterRegistry)
        {
            this.store = IsolatedStorageFile.GetMachineStoreForAssembly();
        }

        protected override void AddOrUpdateValueFunction<T>(string key, T value)
        {
            using (var stream = this.store.OpenFile(key, FileMode.Create, FileAccess.Write))
            {
                using (var sw = new StreamWriter(stream))
                {
                    sw.Write(value);
                }
            }
        }



        protected override T GetValueOrDefaultFunction<T>(string key, T defaultValue)
        {
            T value = defaultValue;

            if (this.store.FileExists(key))
            {
                using (var stream = this.store.OpenFile(key, FileMode.Open))
                {
                    using (var sr = new StreamReader(stream))
                    {
                        var settingsValue = sr.ReadToEnd();
                        if (settingsValue != null)
                        {
                            value = this.TryConvert(settingsValue, defaultValue);
                        }
                    }
                }
            }

            return value;
        }

        ////public void Remove(string key)
        ////{
        ////    lock (this.locker)
        ////    {
        ////        if (this.store.FileExists(key))
        ////        {
        ////            this.store.DeleteFile(key);
        ////        }
        ////    }
        ////}
    }
}